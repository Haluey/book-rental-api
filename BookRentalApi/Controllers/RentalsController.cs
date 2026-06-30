using BookRentalApi.Models;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;

namespace BookRentalApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]     // localhost:port/api/rentals
    public class RentalsController : ControllerBase
    {
        readonly string connString;

        public RentalsController(IConfiguration configuration)
        {
            connString = configuration.GetConnectionString("BookRentalDbConnection")!;
        }

        /// <summary>
        /// 대여 이력 목록 조회
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetRentalsAsync()
        {
            List<Rental> rentals = new();

            using var conn = new MySqlConnection(connString);
            await conn.OpenAsync();

            string query =
                """
                SELECT rental_idx, member_idx, book_idx, rentalDate, returnDate
                  FROM rentals
                 ORDER BY rental_idx DESC
                """;

            using var cmd = new MySqlCommand(query, conn);
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                Rental rental = new()
                {
                    RentalIdx = reader.GetInt32("rental_idx"),
                    MemberIdx = reader.GetInt32("member_idx"),
                    BookIdx = reader.GetInt32("book_idx"),
                    RentalDate = reader.GetDateTime("rentalDate"),
                    ReturnDate = reader.IsDBNull(reader.GetOrdinal("returnDate"))
                        ? null
                        : reader.GetDateTime("returnDate")
                };

                rentals.Add(rental);
            }

            return Ok(rentals);
        }

        /// <summary>
        /// 대여 이력 상세 목록 조회
        /// </summary>
        /// <returns></returns>
        [HttpGet("details")]   // GET /api/rentals/details
        public async Task<IActionResult> GetRentalDetailsAsync()
        {
            List<RentalDetail> rentals = new();

            using var conn = new MySqlConnection(connString);
            await conn.OpenAsync();

            string query =
                """
                SELECT 
                    r.rental_idx,
                    r.member_idx,
                    m.member_name,
                    r.book_idx,
                    b.book_name,
                    r.rentalDate,
                    r.returnDate,
                    CASE
                        WHEN r.returnDate IS NULL THEN '대여중'
                        ELSE '반납완료'
                    END AS status
                  FROM rentals r
                  JOIN members m
                    ON r.member_idx = m.member_idx
                  JOIN books b
                    ON r.book_idx = b.book_idx
                 ORDER BY r.rental_idx DESC
                """;

            using var cmd = new MySqlCommand(query, conn);
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                RentalDetail rental = new()
                {
                    RentalIdx = reader.GetInt32("rental_idx"),

                    MemberIdx = reader.GetInt32("member_idx"),
                    MemberName = reader.GetString("member_name"),

                    BookIdx = reader.GetInt32("book_idx"),
                    BookName = reader.GetString("book_name"),

                    RentalDate = reader.GetDateTime("rentalDate"),
                    ReturnDate = reader.IsDBNull(reader.GetOrdinal("returnDate"))
                        ? null
                        : reader.GetDateTime("returnDate"),

                    Status = reader.GetString("status")
                };

                rentals.Add(rental);
            }

            return Ok(rentals);
        }

        /// <summary>
        /// 대여 이력 단건 조회
        /// </summary>
        /// <param name="id">조회할 대여 번호</param>
        /// <returns></returns>
        [HttpGet("{id}")]   // GET /api/rentals/{id}
        public async Task<IActionResult> GetRentalAsync(int id)
        {
            using var conn = new MySqlConnection(connString);
            await conn.OpenAsync();

            string query =
                """
                SELECT rental_idx, member_idx, book_idx, rentalDate, returnDate
                  FROM rentals
                 WHERE rental_idx = @RentalIdx
                """;

            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@RentalIdx", id);

            using var reader = await cmd.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
            {
                return NotFound($"대여 번호 {id}를 찾을 수 없습니다.");
            }

            Rental rental = new()
            {
                RentalIdx = reader.GetInt32("rental_idx"),
                MemberIdx = reader.GetInt32("member_idx"),
                BookIdx = reader.GetInt32("book_idx"),
                RentalDate = reader.GetDateTime("rentalDate"),
                ReturnDate = reader.IsDBNull(reader.GetOrdinal("returnDate"))
                    ? null
                    : reader.GetDateTime("returnDate")
            };

            return Ok(rental);
        }

        /// <summary>
        /// 현재 대여 중인 목록 조회
        /// </summary>
        /// <returns></returns>
        [HttpGet("active")]     // GET /api/rentals/active
        public async Task<IActionResult> GetActiveRentalsAsync()
        {
            List<Rental> rentals = new();

            using var conn = new MySqlConnection(connString);
            await conn.OpenAsync();

            string query =
                """
                SELECT rental_idx, member_idx, book_idx, rentalDate, returnDate
                  FROM rentals
                 WHERE returnDate IS NULL
                 ORDER BY rental_idx DESC
                """;

            using var cmd = new MySqlCommand(query, conn);
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                Rental rental = new()
                {
                    RentalIdx = reader.GetInt32("rental_idx"),
                    MemberIdx = reader.GetInt32("member_idx"),
                    BookIdx = reader.GetInt32("book_idx"),
                    RentalDate = reader.GetDateTime("rentalDate"),
                    ReturnDate = null
                };

                rentals.Add(rental);
            }

            return Ok(rentals);
        }

        /// <summary>
        /// 현재 대여 중인 상세 목록 조회
        /// </summary>
        /// <returns></returns>
        [HttpGet("active/details")]   // GET /api/rentals/active/details
        public async Task<IActionResult> GetActiveRentalDetailsAsync()
        {
            List<RentalDetail> rentals = new();

            using var conn = new MySqlConnection(connString);
            await conn.OpenAsync();

            string query =
                """
                SELECT 
                    r.rental_idx,
                    r.member_idx,
                    m.member_name,
                    r.book_idx,
                    b.book_name,
                    r.rentalDate,
                    r.returnDate,
                    '대여중' AS status
                  FROM rentals r
                  JOIN members m
                    ON r.member_idx = m.member_idx
                  JOIN books b
                    ON r.book_idx = b.book_idx
                 WHERE r.returnDate IS NULL
                 ORDER BY r.rental_idx DESC
                """;

            using var cmd = new MySqlCommand(query, conn);
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                RentalDetail rental = new()
                {
                    RentalIdx = reader.GetInt32("rental_idx"),

                    MemberIdx = reader.GetInt32("member_idx"),
                    MemberName = reader.GetString("member_name"),

                    BookIdx = reader.GetInt32("book_idx"),
                    BookName = reader.GetString("book_name"),

                    RentalDate = reader.GetDateTime("rentalDate"),
                    ReturnDate = null,

                    Status = reader.GetString("status")
                };

                rentals.Add(rental);
            }

            return Ok(rentals);
        }

        /// <summary>
        /// 회원별 대여 이력 조회
        /// </summary>
        /// <param name="memberId">조회할 회원 번호</param>
        /// <returns></returns>
        [HttpGet("member/{memberId}")]  // GET /api/rentals/member/{memberId}
        public async Task<IActionResult> GetRentalsByMemberAsync(int memberId)
        {
            List<Rental> rentals = new();

            using var conn = new MySqlConnection(connString);
            await conn.OpenAsync();

            string query =
                """
                SELECT rental_idx, member_idx, book_idx, rentalDate, returnDate
                  FROM rentals
                 WHERE member_idx = @MemberIdx
                 ORDER BY rental_idx DESC
                """;

            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@MemberIdx", memberId);

            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                Rental rental = new()
                {
                    RentalIdx = reader.GetInt32("rental_idx"),
                    MemberIdx = reader.GetInt32("member_idx"),
                    BookIdx = reader.GetInt32("book_idx"),
                    RentalDate = reader.GetDateTime("rentalDate"),
                    ReturnDate = reader.IsDBNull(reader.GetOrdinal("returnDate"))
                        ? null
                        : reader.GetDateTime("returnDate")
                };

                rentals.Add(rental);
            }

            return Ok(rentals);
        }

        /// <summary>
        /// 회원별 상세 대여 이력 조회
        /// </summary>
        /// <param name="memberId">조회할 회원 번호</param>
        /// <returns></returns>
        [HttpGet("member/{memberId}/details")]   // GET /api/rentals/member/{memberId}/details
        public async Task<IActionResult> GetRentalDetailsByMemberAsync(int memberId)
        {
            List<RentalDetail> rentals = new();

            using var conn = new MySqlConnection(connString);
            await conn.OpenAsync();

            string query =
                """
                SELECT 
                    r.rental_idx,
                    r.member_idx,
                    m.member_name,
                    r.book_idx,
                    b.book_name,
                    r.rentalDate,
                    r.returnDate,
                    CASE
                        WHEN r.returnDate IS NULL THEN '대여중'
                        ELSE '반납완료'
                    END AS status
                  FROM rentals r
                  JOIN members m
                    ON r.member_idx = m.member_idx
                  JOIN books b
                    ON r.book_idx = b.book_idx
                 WHERE r.member_idx = @MemberIdx
                 ORDER BY r.rental_idx DESC
                """;

            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@MemberIdx", memberId);

            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                RentalDetail rental = new()
                {
                    RentalIdx = reader.GetInt32("rental_idx"),

                    MemberIdx = reader.GetInt32("member_idx"),
                    MemberName = reader.GetString("member_name"),

                    BookIdx = reader.GetInt32("book_idx"),
                    BookName = reader.GetString("book_name"),

                    RentalDate = reader.GetDateTime("rentalDate"),
                    ReturnDate = reader.IsDBNull(reader.GetOrdinal("returnDate"))
                        ? null
                        : reader.GetDateTime("returnDate"),

                    Status = reader.GetString("status")
                };

                rentals.Add(rental);
            }

            return Ok(rentals);
        }

        /// <summary>
        /// 도서별 대여 이력 조회
        /// </summary>
        /// <param name="bookId">조회할 도서 번호</param>
        /// <returns></returns>
        [HttpGet("book/{bookId}")]   // GET /api/rentals/book/{bookId}
        public async Task<IActionResult> GetRentalsByBookAsync(int bookId)
        {
            List<Rental> rentals = new();

            using var conn = new MySqlConnection(connString);
            await conn.OpenAsync();

            string query =
                """
                SELECT rental_idx, member_idx, book_idx, rentalDate, returnDate
                  FROM rentals
                 WHERE book_idx = @BookIdx
                 ORDER BY rental_idx DESC
                """;

            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@BookIdx", bookId);

            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                Rental rental = new()
                {
                    RentalIdx = reader.GetInt32("rental_idx"),
                    MemberIdx = reader.GetInt32("member_idx"),
                    BookIdx = reader.GetInt32("book_idx"),
                    RentalDate = reader.GetDateTime("rentalDate"),
                    ReturnDate = reader.IsDBNull(reader.GetOrdinal("returnDate"))
                        ? null
                        : reader.GetDateTime("returnDate")
                };

                rentals.Add(rental);
            }

            return Ok(rentals);
        }

        /// <summary>
        /// 도서별 상세 대여 이력 조회
        /// </summary>
        /// <param name="bookId">조회할 도서 번호</param>
        /// <returns></returns>
        [HttpGet("book/{bookId}/details")]   // GET /api/rentals/book/{bookId}/details
        public async Task<IActionResult> GetRentalDetailsByBookAsync(int bookId)
        {
            List<RentalDetail> rentals = new();

            using var conn = new MySqlConnection(connString);
            await conn.OpenAsync();

            string query =
                """
                SELECT 
                    r.rental_idx,
                    r.member_idx,
                    m.member_name,
                    r.book_idx,
                    b.book_name,
                    r.rentalDate,
                    r.returnDate,
                    CASE
                        WHEN r.returnDate IS NULL THEN '대여중'
                        ELSE '반납완료'
                    END AS status
                  FROM rentals r
                  JOIN members m
                    ON r.member_idx = m.member_idx
                  JOIN books b
                    ON r.book_idx = b.book_idx
                 WHERE r.book_idx = @BookIdx
                 ORDER BY r.rental_idx DESC
                """;

            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@BookIdx", bookId);

            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                RentalDetail rental = new()
                {
                    RentalIdx = reader.GetInt32("rental_idx"),

                    MemberIdx = reader.GetInt32("member_idx"),
                    MemberName = reader.GetString("member_name"),

                    BookIdx = reader.GetInt32("book_idx"),
                    BookName = reader.GetString("book_name"),

                    RentalDate = reader.GetDateTime("rentalDate"),
                    ReturnDate = reader.IsDBNull(reader.GetOrdinal("returnDate"))
                        ? null
                        : reader.GetDateTime("returnDate"),

                    Status = reader.GetString("status")
                };

                rentals.Add(rental);
            }

            return Ok(rentals);
        }

        /// <summary>
        /// 도서 대여
        /// </summary>
        /// <param name="rental">대여할 회원 번호와 도서 번호</param>
        /// <returns></returns>
        [HttpPost]  // POST /api/rentals
        public async Task<IActionResult> RentBook(RentalRequest rental)
        {
            using var conn = new MySqlConnection(connString);
            await conn.OpenAsync();

            try
            {
                // 1. 회원 존재 여부 확인
                string memberCheckQuery =
                    """
                    SELECT COUNT(*)
                      FROM members
                     WHERE member_idx = @MemberIdx
                    """;

                using var memberCmd = new MySqlCommand(memberCheckQuery, conn);
                memberCmd.Parameters.AddWithValue("@MemberIdx", rental.MemberIdx);

                int memberCount = Convert.ToInt32(await memberCmd.ExecuteScalarAsync());

                if (memberCount == 0)
                {
                    return NotFound($"회원 번호 {rental.MemberIdx}를 찾을 수 없습니다.");
                }

                // 2. 도서 존재 여부 확인
                string bookCheckQuery =
                    """
                    SELECT COUNT(*)
                      FROM books
                     WHERE book_idx = @BookIdx
                    """;

                using var bookCmd = new MySqlCommand(bookCheckQuery, conn);
                bookCmd.Parameters.AddWithValue("@BookIdx", rental.BookIdx);

                int bookCount = Convert.ToInt32(await bookCmd.ExecuteScalarAsync());

                if (bookCount == 0)
                {
                    return NotFound($"도서 번호 {rental.BookIdx}를 찾을 수 없습니다.");
                }

                // 3. 현재 대여 중인지 확인
                string rentalCheckQuery =
                    """
                    SELECT COUNT(*)
                      FROM rentals
                     WHERE book_idx = @BookIdx
                       AND returnDate IS NULL
                    """;

                using var rentalCheckCmd = new MySqlCommand(rentalCheckQuery, conn);
                rentalCheckCmd.Parameters.AddWithValue("@BookIdx", rental.BookIdx);

                int rentalCount = Convert.ToInt32(await rentalCheckCmd.ExecuteScalarAsync());

                if (rentalCount > 0)
                {
                    return BadRequest("이미 대여 중인 도서입니다.");
                }

                // 4. 대여 등록
                string insertQuery =
                    """
                    INSERT INTO rentals
                    (
                        member_idx,
                        book_idx,
                        rentalDate,
                        returnDate
                    )
                    VALUES
                    (
                        @MemberIdx,
                        @BookIdx,
                        CURRENT_DATE,
                        NULL
                    );

                    SELECT LAST_INSERT_ID();
                    """;

                using var insertCmd = new MySqlCommand(insertQuery, conn);
                insertCmd.Parameters.AddWithValue("@MemberIdx", rental.MemberIdx);
                insertCmd.Parameters.AddWithValue("@BookIdx", rental.BookIdx);

                int newId = Convert.ToInt32(await insertCmd.ExecuteScalarAsync());

                return Ok(new
                {
                    rentalIdx = newId,
                    rental.MemberIdx,
                    rental.BookIdx,
                    rentalDate = DateTime.Today,
                    returnDate = (DateTime?)null,
                    message = "도서가 대여되었습니다."
                });
            }
            catch (MySqlException ex)
            {
                return BadRequest(new
                {
                    message = "도서 대여 중 오류가 발생했습니다.",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// 도서 반납
        /// </summary>
        /// <param name="id">반납할 대여 번호</param>
        /// <returns></returns>
        [HttpPut("{id}/return")]    // PUT /api/rentals/{id}/return
        public async Task<IActionResult> ReturnBook(int id)
        {
            using var conn = new MySqlConnection(connString);
            await conn.OpenAsync();

            string query =
                """
                UPDATE rentals
                   SET returnDate = CURRENT_DATE
                 WHERE rental_idx = @RentalIdx
                   AND returnDate IS NULL
                """;

            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@RentalIdx", id);

            int result = await cmd.ExecuteNonQueryAsync();

            if (result == 0)
            {
                return BadRequest("대여 내역이 없거나 이미 반납된 도서입니다.");
            }

            return Ok("도서가 반납되었습니다.");
        }
    }
}
