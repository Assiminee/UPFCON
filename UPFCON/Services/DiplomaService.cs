using UPFCON.Exceptions;
using UPFCON.Interfaces;
using UPFCON.Models;
using UPFCON.Models.DTOs;

namespace UPFCON.Services;

public class DiplomaService(IWebHostEnvironment env) : IDiplomaService
{
    private IWebHostEnvironment Env { get; } = env;
    private const long MaxFileSize  = 5 * 1024 * 1024;
    
    public async Task<Diploma> CreateDiplomaAsync(DiplomaDto diplomaDto)
    {
        if (string.IsNullOrWhiteSpace(diplomaDto.Title))
            throw new InvalidFileException($"Incorrect diploma title: {diplomaDto.Title}");
        
        if (diplomaDto.DiplomaFile.Length == 0)
            throw new InvalidFileException($"Invalid file size: 0Mb");

        var filePath = await SaveDiplomaFileAsync(diplomaDto.DiplomaFile);

        return new Diploma
        {
            Title = diplomaDto.Title,
            IssueDate = diplomaDto.IssueDate,
            Path = filePath,
        };
    }
    
    public async Task<IList<Diploma>> CreateDiplomaListAsync(IList<DiplomaDto> diplomaDtoList)
    {
        IList<Diploma> diplomas = new List<Diploma>();
        
        foreach (var d in diplomaDtoList)
        {
            Diploma diploma = await CreateDiplomaAsync(d);
            diplomas.Add(diploma);
        }
        
        return diplomas;
    }

    private async Task<string> SaveDiplomaFileAsync(IFormFile file)
    {
        var allowedExtensions = new [] {".jpg", ".jpeg", ".png"};
        
        var fileExtension = Path.GetExtension(file.FileName).ToLower();

        if (!allowedExtensions.Contains(fileExtension))
            throw new InvalidFileException($"Invalid file extension: '{fileExtension}'");

        if (file.Length > MaxFileSize)
            throw new InvalidFileException($"Invalid file size: {file.Length}");
        
        var fileName = Guid.NewGuid() + fileExtension;
        
        var uploadsFolder = CreateUploadsDirectoryIfNotExist();
        
        var filePath = Path.Combine(uploadsFolder, fileName);
        using (var fileStream = new FileStream(filePath, FileMode.Create))
            await file.CopyToAsync(fileStream);
        
        return filePath;
    }

    private string CreateUploadsDirectoryIfNotExist()
    {
        var uploadsFolder = Path.Combine(Env.WebRootPath, "uploads", "diplomas");
        
        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);
        
        return uploadsFolder;
    }
}