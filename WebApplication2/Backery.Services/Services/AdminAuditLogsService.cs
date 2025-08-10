using Microsoft.EntityFrameworkCore;
using WebApplication2.DTO;
using WebApplication2.Properties.Data;
using WebApplication2.Properties.Models;
using WebApplication2.Properties.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication2.Properties.Services
{
    // ResultService class
    public class ResultsService<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }
        public List<string>? Errors { get; set; }
        public int StatusCode { get; set; }

        public static ResultsService<T> GoodResult(
            string message,
            int statusCode,
            T? data = default) => new ResultsService<T>
            {
                Message = message,
                StatusCode = statusCode,
                Success = true,
                Data = data
            };

        public static ResultsService<T> BadResult(
            string message,
            int statusCode,
            List<string>? errors = null,
            T? data = default) => new ResultsService<T>
            {
                Message = message,
                StatusCode = statusCode,
                Success = false,
                Errors = errors ?? new List<string>(),
                Data = data
            };
    }

    public class AdminAuditLogsService : IAdminAuditLogsService
    {
        private readonly AppDbContext _context;

        public AdminAuditLogsService(AppDbContext context)
        {
            _context = context;
        }

        // Metody DTO - zaktualizowane z wzorcem ResultService
        public async Task<ResultsService<List<AdminAuditLogListItemDto>>> GetAdminAuditLogsDtoAsync()
        {
            try
            {
                var logs = await _context.AdminAuditLogs
                    .Include(log => log.Admin)
                    .Select(log => new AdminAuditLogListItemDto
                    {
                        LogId = log.LogId,
                        AdminId = log.AdminId,
                        Action = log.Action,
                        TargetType = log.TargetType,
                        TargetId = log.TargetId,
                        Description = log.Description,
                        IpAddress = log.IpAddress,
                        CreatedAt = log.CreatedAt,
                        AdminUsername = log.Admin.Name,
                        AdminEmail = log.Admin.Email
                    })
                    .ToListAsync();

                return ResultsService<List<AdminAuditLogListItemDto>>.GoodResult(
                    "Logi audytu zostały pobrane pomyślnie",
                    200,
                    logs
                );
            }
            catch (Exception ex)
            {
                return ResultsService<List<AdminAuditLogListItemDto>>.BadResult(
                    "Błąd podczas pobierania logów audytu",
                    500,
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ResultsService<AdminAuditLogResponseDto>> GetAdminAuditLogByIdDtoAsync(int id)
        {
            try
            {
                var log = await _context.AdminAuditLogs
                    .Include(l => l.Admin)
                    .Where(l => l.LogId == id)
                    .Select(log => new AdminAuditLogResponseDto
                    {
                        LogId = log.LogId,
                        AdminId = log.AdminId,
                        Action = log.Action,
                        TargetType = log.TargetType,
                        TargetId = log.TargetId,
                        Description = log.Description,
                        IpAddress = log.IpAddress,
                        UserAgent = log.UserAgent,
                        CreatedAt = log.CreatedAt,
                        Admin = new AdminDto
                        {
                            AdminId = log.Admin.AdminId,
                            Username = log.Admin.Name,
                            Email = log.Admin.Email,
                        }
                    })
                    .FirstOrDefaultAsync();

                if (log == null)
                {
                    return ResultsService<AdminAuditLogResponseDto>.BadResult(
                        "Nie znaleziono logu audytu o podanym ID",
                        404
                    );
                }

                return ResultsService<AdminAuditLogResponseDto>.GoodResult(
                    "Log audytu został pobrany pomyślnie",
                    200,
                    log
                );
            }
            catch (Exception ex)
            {
                return ResultsService<AdminAuditLogResponseDto>.BadResult(
                    "Błąd podczas pobierania logu audytu",
                    500,
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ResultsService<AdminAuditLogResponseDto>> CreateAdminAuditLogAsync(CreateAdminAuditLogDto createDto)
        {
            try
            {
                var log = new AdminAuditLog
                {
                    AdminId = createDto.AdminId,
                    Action = createDto.Action,
                    TargetType = createDto.TargetType,
                    TargetId = createDto.TargetId,
                    Description = createDto.Description,
                    IpAddress = createDto.IpAddress,
                    UserAgent = createDto.UserAgent,
                    CreatedAt = DateTime.Now
                };

                _context.AdminAuditLogs.Add(log);
                await _context.SaveChangesAsync();

                var createdLogResult = await GetAdminAuditLogByIdDtoAsync(log.LogId);
                if (!createdLogResult.Success)
                {
                    return ResultsService<AdminAuditLogResponseDto>.BadResult(
                        "Błąd podczas pobierania utworzonego logu audytu",
                        500
                    );
                }

                return ResultsService<AdminAuditLogResponseDto>.GoodResult(
                    "Log audytu został utworzony pomyślnie",
                    201,
                    createdLogResult.Data
                );
            }
            catch (Exception ex)
            {
                return ResultsService<AdminAuditLogResponseDto>.BadResult(
                    "Błąd podczas tworzenia logu audytu",
                    500,
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ResultsService<object>> UpdateAdminAuditLogAsync(int id, UpdateAdminAuditLogDto updateDto)
        {
            try
            {
                var existingLog = await _context.AdminAuditLogs.FindAsync(id);
                if (existingLog == null)
                {
                    return ResultsService<object>.BadResult(
                        "Nie znaleziono logu audytu o podanym ID",
                        404
                    );
                }

                existingLog.AdminId = updateDto.AdminId;
                existingLog.Action = updateDto.Action;
                existingLog.TargetType = updateDto.TargetType;
                existingLog.TargetId = updateDto.TargetId;
                existingLog.Description = updateDto.Description;
                existingLog.IpAddress = updateDto.IpAddress;
                existingLog.UserAgent = updateDto.UserAgent;

                await _context.SaveChangesAsync();

                return ResultsService<object>.GoodResult(
                    "Log audytu został zaktualizowany pomyślnie",
                    204
                );
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await AdminAuditLogExistsAsync(id))
                {
                    return ResultsService<object>.BadResult(
                        "Nie znaleziono logu audytu o podanym ID",
                        404
                    );
                }

                return ResultsService<object>.BadResult(
                    "Błąd współbieżności podczas aktualizacji logu audytu",
                    409
                );
            }
            catch (Exception ex)
            {
                return ResultsService<object>.BadResult(
                    "Błąd podczas aktualizacji logu audytu",
                    500,
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ResultsService<object>> DeleteAdminAuditLogAsync(int id)
        {
            try
            {
                var log = await _context.AdminAuditLogs.FindAsync(id);
                if (log == null)
                {
                    return ResultsService<object>.BadResult(
                        "Nie znaleziono logu audytu o podanym ID",
                        404
                    );
                }

                _context.AdminAuditLogs.Remove(log);
                await _context.SaveChangesAsync();

                return ResultsService<object>.GoodResult(
                    "Log audytu został usunięty pomyślnie",
                    204
                );
            }
            catch (Exception ex)
            {
                return ResultsService<object>.BadResult(
                    "Błąd podczas usuwania logu audytu",
                    500,
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<bool> AdminAuditLogExistsAsync(int id)
        {
            return await _context.AdminAuditLogs.AnyAsync(e => e.LogId == id);
        }

        // Oryginalne metody (zachowane dla kompatybilności wstecznej)
        public async Task<List<AdminAuditLog>> GetAdminAuditLogsAsync()
        {
            return await _context.AdminAuditLogs
                .Include(log => log.Admin)
                .ToListAsync();
        }

        public async Task<AdminAuditLog> GetAdminAuditLogByIdAsync(int id)
        {
            return await _context.AdminAuditLogs
                .Include(l => l.Admin)
                .FirstOrDefaultAsync(l => l.LogId == id);
        }

        public async Task<AdminAuditLog> CreateAdminAuditLogAsync(AdminAuditLog log)
        {
            _context.AdminAuditLogs.Add(log);
            await _context.SaveChangesAsync();
            return log;
        }

        public async Task<bool> UpdateAdminAuditLogAsync(int id, AdminAuditLog log)
        {
            if (id != log.LogId)
                return false;

            _context.Entry(log).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await AdminAuditLogExistsAsync(id))
                    return false;
                throw;
            }
        }
    }
}