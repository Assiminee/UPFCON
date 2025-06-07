using UPFCON.Models;
using UPFCON.Models.DTOs;

namespace UPFCON.Interfaces;

public interface IDiplomaService
{
    Task<Diploma> CreateDiplomaAsync(DiplomaDto diplomaDto);
    Task<IList<Diploma>> CreateDiplomaListAsync(IList<DiplomaDto> diplomaDtoList);
}