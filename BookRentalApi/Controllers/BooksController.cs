using BookRentalApi.Models;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;

namespace BookRentalApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]     // localhost:port/api/books
    public class BooksController : ControllerBase
    {
        readonly string connString;

        public BooksController(IConfiguration configuration)
        {
            connString = configuration.GetConnectionString("BookRentalDbConnection")!;
        }

        /// <summary>
        /// 대여 가능한 도서 목록 조회
        /// </summary>
        /// <returns></returns>
        [HttpGet("available")]   // GET /api/books/available
        public async Task<IActionResult> GetAvailableBooksAsync()
        {
            List<Book> books = new();

            using var conn = new MySqlConnection(connString);
            await conn.OpenAsync();

            string query =
                """
                SELECT book_idx, author, div_code, book_name, release_dt, isbn, price
                  FROM books
                 WHERE book_idx NOT IN
                 (
                     SELECT book_idx
                       FROM rentals
                      WHERE returnDate IS NULL
                 )
                 ORDER BY book_idx DESC
                """;

            using var cmd = new MySqlCommand(query, conn);
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                Book book = new()
                {
                    BookIdx = reader.GetInt32("book_idx"),
                    Author = reader.GetString("author"),
                    DivCode = reader.GetString("div_code"),
                    BookName = reader.GetString("book_name"),
                    ReleaseDt = reader.GetDateTime("release_dt"),
                    Isbn = reader.GetString("isbn"),
                    Price = reader.GetDecimal("price")
                };

                books.Add(book);
            }

            return Ok(books);
        }

        /// <summary>
        /// 도서 목록 조회
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetBooksAsync()
        {
            List<Book> books = new();

            using var conn = new MySqlConnection(connString);
            await conn.OpenAsync();

            string query =
                """
                SELECT book_idx, author, div_code, book_name, release_dt, isbn, price
                  FROM books
                 ORDER BY book_idx DESC
                """;

            using var cmd = new MySqlCommand(query, conn);
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                Book book = new()
                {
                    BookIdx = reader.GetInt32("book_idx"),
                    Author = reader.GetString("author"),
                    DivCode = reader.GetString("div_code"),
                    BookName = reader.GetString("book_name"),
                    ReleaseDt = reader.GetDateTime("release_dt"),
                    Isbn = reader.GetString("isbn"),
                    Price = reader.GetDecimal("price")
                };

                books.Add(book);
            }

            return Ok(books);
        }

        /// <summary>
        /// 도서 단건 조회
        /// </summary>
        /// <param name="id">도서 번호</param>
        /// <returns></returns>
        [HttpGet("{id}")]   // GET /api/Books/{id}
        public async Task<IActionResult> GetBookAsync(int id)
        {
            using var conn = new MySqlConnection(connString);
            await conn.OpenAsync();

            string query =
                """
                SELECT book_idx, author, div_code, book_name, release_dt, isbn, price
                  FROM books
                 WHERE book_idx = @BookIdx
                """;

            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@BookIdx", id);

            using var reader = await cmd.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
            {
                return NotFound($"도서 번호 {id}를 찾을 수 없습니다.");
            }

            Book book = new()
            {
                BookIdx = reader.GetInt32("book_idx"),
                Author = reader.GetString("author"),
                DivCode = reader.GetString("div_code"),
                BookName = reader.GetString("book_name"),
                ReleaseDt = reader.GetDateTime("release_dt"),
                Isbn = reader.GetString("isbn"),
                Price = reader.GetDecimal("price")
            };

            return Ok(book);
        }

        /// <summary>
        /// 도서 등록
        /// </summary>
        /// <param name="book">도서 정보</param>
        /// <returns></returns>
        [HttpPost]  // POST /api/books
        public async Task<IActionResult> CreateBook(BookRequest book)
        {
            using var conn = new MySqlConnection(connString);
            await conn.OpenAsync();

            string query =
                """
                INSERT INTO books
                (
                    author,
                    div_code,
                    book_name,
                    release_dt,
                    isbn,
                    price
                )
                VALUES
                (
                    @Author,
                    @DivCode,
                    @BookName,
                    @ReleaseDt,
                    @Isbn,
                    @Price
                );

                SELECT LAST_INSERT_ID();
                """;

            using var cmd = new MySqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@Author", book.Author);
            cmd.Parameters.AddWithValue("@DivCode", book.DivCode);
            cmd.Parameters.AddWithValue("@BookName", book.BookName);
            cmd.Parameters.AddWithValue("@ReleaseDt", book.ReleaseDt);
            cmd.Parameters.AddWithValue("@Isbn", book.Isbn);
            cmd.Parameters.AddWithValue("@Price", book.Price);

            try
            {
                int newId = Convert.ToInt32(await cmd.ExecuteScalarAsync());

                return Ok(new
                {
                    bookIdx = newId,
                    book.Author,
                    book.DivCode,
                    book.BookName,
                    book.ReleaseDt,
                    book.Isbn,
                    book.Price
                });
            }
            catch (MySqlException ex)
            {
                return BadRequest(new
                {
                    message = "도서 등록 중 오류가 발생했습니다.",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// 도서 수정
        /// </summary>
        /// <param name="id">도서 번호</param>
        /// <param name="book">도서 수정 정보</param>
        /// <returns></returns>
        [HttpPut("{id}")]   // PUT /api/books/{id}
        public async Task<IActionResult> UpdateBook(int id, BookRequest book)
        {
            using var conn = new MySqlConnection(connString);
            await conn.OpenAsync();

            string query =
                """
                UPDATE books
                   SET author = @Author,
                       div_code = @DivCode,
                       book_name = @BookName,
                       release_dt = @ReleaseDt,
                       isbn = @Isbn,
                       price = @Price
                 WHERE book_idx = @BookIdx;
                """;

            using var cmd = new MySqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@Author", book.Author);
            cmd.Parameters.AddWithValue("@DivCode", book.DivCode);
            cmd.Parameters.AddWithValue("@BookName", book.BookName);
            cmd.Parameters.AddWithValue("@ReleaseDt", book.ReleaseDt);
            cmd.Parameters.AddWithValue("@Isbn", book.Isbn);
            cmd.Parameters.AddWithValue("@Price", book.Price);
            cmd.Parameters.AddWithValue("@BookIdx", id);

            try
            {
                int result = await cmd.ExecuteNonQueryAsync();

                if (result == 0)
                {
                    return NotFound($"도서 번호 {id}를 찾을 수 없습니다.");
                }

                return Ok("도서 정보가 수정되었습니다.");
            }
            catch (MySqlException ex)
            {
                return BadRequest(new
                {
                    message = "도서 수정 중 오류가 발생했습니다.",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// 도서 삭제
        /// </summary>
        /// <param name="id">도서 번호</param>
        /// <returns></returns>
        [HttpDelete("{id}")] // DELETE /api/books/{id}
        public async Task<IActionResult> DeleteBook(int id)
        {
            using var conn = new MySqlConnection(connString);
            await conn.OpenAsync();

            string query =
                """
                DELETE FROM books
                 WHERE book_idx = @BookIdx;
                """;

            using var cmd = new MySqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@BookIdx", id);

            try
            {
                int result = await cmd.ExecuteNonQueryAsync();

                if (result == 0)
                {
                    return NotFound($"도서 번호 {id}를 찾을 수 없습니다.");
                }

                return Ok("도서가 삭제되었습니다.");
            }
            catch (MySqlException ex)
            {
                return BadRequest(new
                {
                    message = "도서 삭제 중 오류가 발생했습니다.",
                    error = ex.Message
                });
            }
        }
    }
}
