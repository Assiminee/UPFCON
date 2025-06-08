using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace UPFCON.Models.Context;

public class UpfconContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public UpfconContext()
    {
    }

    public UpfconContext(DbContextOptions<UpfconContext> options)
        : base(options)
    {
    }

    public required DbSet<Admin> Admins { get; set; }
    public required DbSet<Attendee> Attendees { get; set; }
    public required DbSet<Author> Authors { get; set; }
    public required DbSet<BoardDirector> BoardDirectors { get; set; }
    public required DbSet<BoardDirectorDecision> BoardDirectorDecisions { get; set; }
    public required DbSet<Chairman> Chairmans { get; set; }
    public required DbSet<CommitteeMember> CommitteeMembers { get; set; }
    public required DbSet<Contribution> Contributions { get; set; }
    public required DbSet<Diploma> Diplomas { get; set; }
    public required DbSet<Evaluation> Evaluations { get; set; }
    public required DbSet<Event> Events { get; set; }
    public required DbSet<Paper> Papers { get; set; }
    public required DbSet<Attendance> Attendances { get; set; }
    public required DbSet<SubmissionRules> SubmissionRules { get; set; }
    public required DbSet<TimeSlot> TimeSlots { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Defines relationships between entities:
        UserSetUp(modelBuilder);
        DiplomaSetup(modelBuilder);
        BoardDirectorSetup(modelBuilder);
        BoardDirectorDecisionSetup(modelBuilder);
        AttendeeSetup(modelBuilder);
        AttendanceSetup(modelBuilder);
        ChairmanSetup(modelBuilder);
        CommitteeMemberSetup(modelBuilder);
        EvaluationSetup(modelBuilder);
        PaperSetup(modelBuilder);
        AuthorSetup(modelBuilder);
        ContributionSetup(modelBuilder);
        TimeSlotSetup(modelBuilder);
        EventSetup(modelBuilder);
    }

    private static void UserSetUp(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(u =>
        {
            u.HasIndex(us => us.NormalizedEmail)
                .IsUnique();

            u.Property(us => us.AccountStatus)
                .HasMaxLength(50)
                .HasDefaultValue(AccountStatus.PendingVerification)
                .IsRequired();

            u.ToTable(us => us.HasCheckConstraint(
                "CK_AllowedValuesAccountStatus",
                "AccountStatus IN ('Verified','PendingVerification','Rejected','Deleted')"
            ));

            u.ToTable("Users");
        });

        modelBuilder.Entity<Admin>().ToTable("Admins");
        modelBuilder.Entity<BoardDirector>().ToTable("BoardDirectors");
        modelBuilder.Entity<IdentityRole<Guid>>().ToTable("Roles");
        modelBuilder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");
        modelBuilder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
        modelBuilder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
        modelBuilder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");
        modelBuilder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
    }

    private static void DiplomaSetup(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Diploma>(d =>
        {
            d.HasKey(dp => dp.Id);

            d.Property(dp => dp.Id)
                .HasDefaultValueSql("NEWID()");

            d.HasOne(dp => dp.User)
                .WithMany(u => u.Diplomas)
                .HasForeignKey(dp => dp.UserId);

            d.HasOne(dp => dp.VerifiedBy)
                .WithMany(a => a.VerifiedDiplomas)
                .HasForeignKey(dp => dp.AdminId);

            d.Property(dp => dp.VerificationStatus)
                .HasMaxLength(50)
                .HasDefaultValue(DiplomaVerificationStatus.PendingVerification)
                .IsRequired();

            d.ToTable(dp => dp.HasCheckConstraint(
                "CK_AllowedValuesDiplomaVerificationStatus",
                "VerificationStatus IN ('Verified','PendingVerification','Rejected')"
            ));
        });
    }

    private static void BoardDirectorDecisionSetup(ModelBuilder modelBuilder)
    {
        /*
         * Defining a composite key:
         * BoardDirectorDecision's primary key is made up
         * of both the BoardDirector's ID and the Event's ID
         *
         * Defining a * -> 1 -> * relationship (or * -> * relationship
         * with a join table defined manually)
         */
        modelBuilder.Entity<BoardDirectorDecision>(bdd =>
        {
            bdd.HasKey(b => new { b.BoardDirectorId, b.EventId });

            bdd.HasOne(b => b.BoardDirector)
                .WithMany(bd => bd.Decisions)
                .HasForeignKey(b => b.BoardDirectorId);

            bdd.HasOne(b => b.Event)
                .WithMany(e => e.BoardDecisions)
                .HasForeignKey(b => b.EventId);

            bdd.Property(bd => bd.ApprovalStatus)
                .HasMaxLength(50)
                .HasDefaultValue(ApprovalStatusEnum.PendingDecision)
                .IsRequired();

            bdd.ToTable(bd => bd.HasCheckConstraint(
                "CK_ApprovalStatusAllowedValues",
                "ApprovalStatus IN ('PendingDecision','ToBeRevised','Rejected','Approved')"
            ));
        });
    }

    private static void BoardDirectorSetup(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BoardDirector>(bd =>
        {
            bd.Property(b => b.Role)
                .HasMaxLength(50)
                .IsRequired();

            bd.ToTable(b => b.HasCheckConstraint(
                "CK_AllowedBoardDirectorRole",
                "Role IN ('President','VicePresident','Dean')"
            ));
        });
    }

    private static void AttendeeSetup(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Attendee>(a =>
        {
            a.HasKey(att => att.UserId);

            a.HasOne(att => att.User)
                .WithOne(u => u.Attendee)
                .HasForeignKey<Attendee>(att => att.UserId);

            a.HasMany(att => att.EventsAttended)
                .WithOne(attendance => attendance.Attendee)
                .HasForeignKey(attendance => attendance.AttendeeId)
                .OnDelete(DeleteBehavior.ClientCascade);
        });
    }

    private static void AttendanceSetup(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Attendance>(a =>
        {
            a.HasKey(att => new { att.AttendeeId, att.EventId });

            a.Property(att => att.IsExpert)
                .HasDefaultValue(false);
        });
    }

    private static void ChairmanSetup(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Chairman>(c =>
        {
            c.HasKey(ch => ch.UserId);

            c.HasOne(ch => ch.User)
                .WithOne(u => u.Chairman)
                .HasForeignKey<Chairman>(ch => ch.UserId);

            c.Property(ch => ch.IsInternal)
                .HasDefaultValue(true);

            c.HasMany(ch => ch.Memberships)
                .WithOne(cm => cm.Chairman)
                .HasForeignKey(cm => cm.ChairmanId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }


    private static void CommitteeMemberSetup(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CommitteeMember>(cm =>
        {
            cm.HasKey(c => new { c.ChairmanId, c.EventId });

            cm.Property(c => c.Role)
                .HasMaxLength(50)
                .IsRequired()
                .HasDefaultValue(CommitteeMemberRole.Evaluator);

            cm.ToTable(c => c.HasCheckConstraint(
                "CK_AllowedCommitteeMemberRoles",
                "Role IN ('ExternalOrganizerChairman','Evaluator','HeadChairman')"
            ));

            cm.Property(c => c.InvitationStatus)
                .HasMaxLength(50)
                .IsRequired()
                .HasDefaultValue(InvitationStatusEnum.PendingResponse);

            cm.ToTable(c => c.HasCheckConstraint(
                "CK_AllowedInvitationStatuses",
                "InvitationStatus IN ('Rejected','PendingResponse','Accepted')"
            ));

            cm.HasMany(c => c.Evaluations)
                .WithOne(e => e.Evaluator)
                .HasForeignKey(e => new { e.EvaluatorId, e.EventId })
                .OnDelete(DeleteBehavior.ClientCascade);
        });
    }

    private static void EvaluationSetup(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Evaluation>(e =>
        {
            e.HasKey(eval => eval.Id);

            e.Property(eval => eval.Id)
                .HasDefaultValueSql("NEWID()");
        });
    }

    private static void PaperSetup(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Paper>(p =>
        {
            p.HasKey(pr => pr.Id);

            p.Property(pr => pr.Id)
                .HasDefaultValueSql("NEWID()");

            p.Property(pr => pr.Status)
                .HasMaxLength(50)
                .IsRequired()
                .HasDefaultValue(PaperStatus.PendingEvaluation);

            p.ToTable(pr => pr.HasCheckConstraint(
                "CK_AllowedPaperStatuses",
                "Status IN ('RequiresEdits','Reject','Accepted','PendingEvaluation')"
            ));

            p.HasMany(pr => pr.Evaluations)
                .WithOne(e => e.Paper)
                .HasForeignKey(e => e.PaperId)
                .OnDelete(DeleteBehavior.Cascade);

            p.HasMany(pr => pr.Contributors)
                .WithOne(c => c.Paper)
                .HasForeignKey(c => c.PaperId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void AuthorSetup(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Author>(a =>
        {
            a.HasKey(ath => ath.UserId);

            a.HasOne(ath => ath.User)
                .WithOne(u => u.Author)
                .HasForeignKey<Author>(u => u.UserId);

            a.HasMany(ath => ath.Contributions)
                .WithOne(c => c.Author)
                .HasForeignKey(c => c.AuthorId)
                .OnDelete(DeleteBehavior.ClientCascade);
        });
    }

    private static void ContributionSetup(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Contribution>(c =>
        {
            c.HasKey(cn => new { cn.AuthorId, cn.PaperId });

            c.Property(cn => cn.Role)
                .HasMaxLength(50)
                .IsRequired()
                .HasDefaultValue(ContributorRole.Contributor);

            c.ToTable(cn => cn.HasCheckConstraint(
                "CK_AllowedContributorRoles",
                "Role IN ('Contributor','HeadAuthor')"
            ));
        });
    }

    private static void EventSetup(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Event>(e =>
        {
            e.HasKey(ev => ev.Id);

            e.Property(ev => ev.Id)
                .HasDefaultValueSql("NEWID()");

            e.HasMany(ev => ev.Attendees)
                .WithOne(att => att.Event)
                .HasForeignKey(att => att.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasMany(ev => ev.SubmittedPapers)
                .WithOne(p => p.Event)
                .HasForeignKey(p => p.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasMany(ev => ev.TimeSlots)
                .WithOne(ts => ts.Event)
                .HasForeignKey(ts => ts.EventId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }


    private static void TimeSlotSetup(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TimeSlot>()
            .HasKey(ts => new { ts.EventId, ts.PaperId });

        modelBuilder.Entity<TimeSlot>()
            .HasOne(ts => ts.Paper)
            .WithOne(ts => ts.TimeSlot)
            .HasForeignKey<TimeSlot>(ts => ts.PaperId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TimeSlot>()
            .HasOne(ts => ts.Event)
            .WithMany(e => e.TimeSlots)
            .HasForeignKey(ts => ts.EventId)
            .OnDelete(DeleteBehavior.Restrict);
    }
    // Use this to connect to data source:
    // Server=(localdb)\MSSQLLocalDB;Database=UPFCON;Integrated Security=True;TrustServerCertificate=True;
}