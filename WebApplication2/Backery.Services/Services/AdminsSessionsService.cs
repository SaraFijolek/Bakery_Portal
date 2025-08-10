using Microsoft.EntityFrameworkCore;
using WebApplication2.DTO;
using WebApplication2.Properties.Data;
using WebApplication2.Properties.DTOs;
using WebApplication2.Properties.Models;
using WebApplication2.Properties.Services.Interfaces;
using static WebApplication2.Properties.Services.AdminsSessionsService;

namespace WebApplication2.Properties.Services
{
    public class AdminsSessionsService : IAdminsSessionsService
    {
        public class AdminSessionService<T>
        {
            public bool Success { get; set; }
            public T? Data { get; set; }
            public string? Message { get; set; }
            public List<string>? Errors { get; set; }
            public int StatusCode { get; set; }

            public static AdminSessionService<T> GoodResult(
                string message,
                int statusCode,
                T? data = default)
                => new AdminSessionService<T>
                {
                    Message = message,
                    StatusCode = statusCode,
                    Success = true,
                    Data = data
                };

            public static AdminSessionService<T> BadResult(
                string message,
                int statusCode,
                List<string>? errors = null,
                T? data = default)
                =>
                new AdminSessionService<T>
                {
                    Message = message,
                    StatusCode = statusCode,
                    Success = false,
                    Errors = errors ?? new List<string>(),
                    Data = data
                };
        }


        private readonly AppDbContext _context;

        public AdminsSessionsService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<AdminSessionService<List<AdminSessionReadDto>>> GetAllAdminSessionsAsync()
        {
            try
            {
                var sessions = await _context.AdminSessions
                    .Include(s => s.Admin)
                    .ToListAsync();

                var sessionDtos = AdminSessionMapper.ToDtoList(sessions);

                return AdminSessionService<List<AdminSessionReadDto>>.GoodResult(
                    "Admin sessions retrieved successfully",
                    200,
                    sessionDtos);
            }
            catch (Exception ex)
            {
                return AdminSessionService<List<AdminSessionReadDto>>.BadResult(
                    "Error occurred while retrieving admin sessions",
                    500,
                    new List<string> { ex.Message });
            }
        }

        public async Task<AdminSessionService<List<AdminSessionListDto>>> GetAllAdminSessionsListAsync()
        {
            try
            {
                var sessions = await _context.AdminSessions
                    .Include(s => s.Admin)
                    .ToListAsync();

                var sessionListDtos = AdminSessionMapper.ToListDtoList(sessions);

                return AdminSessionService<List<AdminSessionListDto>>.GoodResult(
                    "Admin sessions list retrieved successfully",
                    200,
                    sessionListDtos);
            }
            catch (Exception ex)
            {
                return AdminSessionService<List<AdminSessionListDto>>.BadResult(
                    "Error occurred while retrieving admin sessions list",
                    500,
                    new List<string> { ex.Message });
            }
        }

        public async Task<AdminSessionService<AdminSessionReadDto>> GetAdminSessionByIdAsync(int id)
        {
            try
            {
                var session = await _context.AdminSessions
                    .Include(s => s.Admin)
                    .FirstOrDefaultAsync(s => s.SessionId == id);

                if (session == null)
                {
                    return AdminSessionService<AdminSessionReadDto>.BadResult(
                        $"Admin session with ID {id} not found",
                        404);
                }

                var sessionDto = AdminSessionMapper.ToDto(session);

                return AdminSessionService<AdminSessionReadDto>.GoodResult(
                    "Admin session retrieved successfully",
                    200,
                    sessionDto);
            }
            catch (Exception ex)
            {
                return AdminSessionService<AdminSessionReadDto>.BadResult(
                    "Error occurred while retrieving admin session",
                    500,
                    new List<string> { ex.Message });
            }
        }

        public async Task<AdminSessionService<AdminSessionReadDto>> CreateAdminSessionAsync(CreateAdminSessionDto sessionDto)
        {
            try
            {
                var session = AdminSessionMapper.ToModel(sessionDto);
                _context.AdminSessions.Add(session);
                await _context.SaveChangesAsync();

                // Reload with Admin data
                var createdSession = await _context.AdminSessions
                    .Include(s => s.Admin)
                    .FirstOrDefaultAsync(s => s.SessionId == session.SessionId);

                var createdSessionDto = AdminSessionMapper.ToDto(createdSession);

                return AdminSessionService<AdminSessionReadDto>.GoodResult(
                    "Admin session created successfully",
                    201,
                    createdSessionDto);
            }
            catch (Exception ex)
            {
                return AdminSessionService<AdminSessionReadDto>.BadResult(
                    "Error occurred while creating admin session",
                    500,
                    new List<string> { ex.Message });
            }
        }

        public async Task<AdminSessionService<bool>> UpdateAdminSessionAsync(int id, UpdateAdminSessionDto sessionDto)
        {
            try
            {
                var existingSession = await _context.AdminSessions.FindAsync(id);
                if (existingSession == null)
                {
                    return AdminSessionService<bool>.BadResult(
                        $"Admin session with ID {id} not found",
                        404);
                }

                AdminSessionMapper.UpdateModel(existingSession, sessionDto);

                try
                {
                    await _context.SaveChangesAsync();

                    return AdminSessionService<bool>.GoodResult(
                        "Admin session updated successfully",
                        200,
                        true);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await AdminSessionExistsAsync(id))
                    {
                        return AdminSessionService<bool>.BadResult(
                            $"Admin session with ID {id} not found",
                            404);
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                return AdminSessionService<bool>.BadResult(
                    "Error occurred while updating admin session",
                    500,
                    new List<string> { ex.Message });
            }
        }

        public async Task<AdminSessionService<bool>> DeleteAdminSessionAsync(int id)
        {   
            try
            {
                var session = await _context.AdminSessions.FindAsync(id);
                if (session == null)
                {
                    return AdminSessionService<bool>.BadResult(
                        $"Admin session with ID {id} not found",
                        404);
                }

                _context.AdminSessions.Remove(session);
                await _context.SaveChangesAsync();

                return AdminSessionService<bool>.GoodResult(
                    "Admin session deleted successfully",
                    200,
                    true);
            }
            catch (Exception ex)
            {
                return AdminSessionService<bool>.BadResult(
                    "Error occurred while deleting admin session",
                    500,
                    new List<string> { ex.Message });
            }
        }

        public async Task<bool> AdminSessionExistsAsync(int id)
        {
            return await _context.AdminSessions.AnyAsync(e => e.SessionId == id);
        }
    }
}
