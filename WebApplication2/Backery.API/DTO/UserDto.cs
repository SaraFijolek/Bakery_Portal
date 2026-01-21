using System.ComponentModel.DataAnnotations;


namespace WebApplication2.DTO
{

    public class UserResponseDto
    {
        public string UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
       
        public string? Phone { get; set; }
    }

    public class UserCreateDto
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } 

        [StringLength(15)]
        public string? Phone { get; set; }
    }

    public class UserUpdateDto
    {
        

        [Required]
        [StringLength(50)]
        public string Name { get; set; }    

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }

        [StringLength(15)]
        public string? Phone { get; set; }

        

        
    }

    public class UserQueryDto
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public string? SortBy { get; set; } = "UserId";
        public bool SortDescending { get; set; } = false;
        
    }

    public class PagedResult<T>
    {
        public List<T> Items { get; set; }
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasNextPage => Page < TotalPages;
        public bool HasPreviousPage => Page > 1;
    }
}
