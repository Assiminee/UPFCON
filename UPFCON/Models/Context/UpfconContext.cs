using System.Data.Entity;

namespace UPFCON.Models.Context;

public class UpfconContext : DbContext
{
    public required DbSet<User> Users { get; set; }
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
    public required DbSet<Participation> Participations { get; set; }
    public required DbSet<SubmissionRules> SubmissionRuless { get; set; }
    public required DbSet<TimeSlot> TimeSlots { get; set; }

    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        BoardDirectorDecisionMapping(modelBuilder);
        ContributionMapping(modelBuilder);
        CommitteeMemberMapping(modelBuilder);
        ParticipationMapping(modelBuilder);

        modelBuilder.Entity<TimeSlot>()
            .HasKey(ts => new { ts.EventId, ts.PaperId });

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Phone)
            .IsUnique();
    }

    private static void BoardDirectorDecisionMapping(DbModelBuilder modelBuilder)
    {
        /*
         * Defining a composite key:
         * BoardDirectorDecision's primary key is made up
         * of both the BoardDirector's Id and the Event's Id
         */
        modelBuilder.Entity<BoardDirectorDecision>()
            .HasKey(bdd => new { bdd.BoardDirectorId, bdd.EventId });

        /*
         * Defining a * -> 1 -> * relationship (or * -> * relationship
         * with a join table defined manually)
         */
        modelBuilder.Entity<BoardDirectorDecision>()
            .HasRequired(bdd => bdd.BoardDirector)
            .WithMany(bd => bd.Decisions)
            .HasForeignKey(bdd => bdd.BoardDirectorId);
        
        modelBuilder.Entity<BoardDirectorDecision>()
            .HasRequired(bdd => bdd.Event)
            .WithMany(e => e.BoardDecisions)
            .HasForeignKey(bdd => bdd.EventId);
    }

    private static void ContributionMapping(DbModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Contribution>()
            .HasKey(c => new { c.AuthorId, c.PaperId });
        
        modelBuilder.Entity<Contribution>()
            .HasRequired(c => c.Author)
            .WithMany(a => a.Contributions)
            .HasForeignKey(c => c.AuthorId);
        
        modelBuilder.Entity<Contribution>()
            .HasRequired(c => c.Paper)
            .WithMany(p => p.Contributors)
            .HasForeignKey(c => c.PaperId);
    }

    private static void CommitteeMemberMapping(DbModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CommitteeMember>()
            .HasKey(cm => new {cm.ChairmanId, cm.EventId });

        modelBuilder.Entity<CommitteeMember>()
            .HasRequired(cm => cm.Chairman)
            .WithMany(c => c.Memberships)
            .HasForeignKey(cm => cm.ChairmanId);
        
        modelBuilder.Entity<CommitteeMember>()
            .HasRequired(cm => cm.Event)
            .WithMany(e => e.OrganizingCommittee)
            .HasForeignKey(cm => cm.EventId);
    }

    private static void ParticipationMapping(DbModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Participation>()
            .HasKey(p => new { p.AttendeeId, p.EventId });
        
        modelBuilder.Entity<Participation>()
            .HasRequired(p => p.Attendee)
            .WithMany(a => a.Events)
            .HasForeignKey(p => p.AttendeeId);
        
        modelBuilder.Entity<Participation>()
            .HasRequired(p => p.Event)
            .WithMany(e => e.Attendees)
            .HasForeignKey(p => p.EventId);
    }

    
}