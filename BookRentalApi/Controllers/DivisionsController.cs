using BookRentalApi.Models;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;

namespace BookRentalApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]     // localhost:port/api/divisions
    public class DivisionsController : ControllerBase
    {
        readonly string connString;

        public DivisionsController(IConfiguration configuration)
        {
            connString = configuration.GetConnectionString("BookRentalDbConnection")!;
        }

        /// <summary>
        /// 도서 분류 목록 조회
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetDivisionsAsync()
        {
            List<Division> divisions = new();

            using var conn = new MySqlConnection(connString);
            await conn.OpenAsync();

            string query =
                """
                SELECT div_code, div_name
                  FROM division
                 ORDER BY div_code
                """;

            using var cmd = new MySqlCommand(query, conn);
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                Division division = new()
                {
                    DivCode = reader.GetString("div_code"),
                    DivName = reader.GetString("div_name")
                };

                divisions.Add(division);
            }

            return Ok(divisions);
        }

        /// <summary>
        /// 도서 분류 단건 조회
        /// </summary>
        /// <param name="divCode">도서 분류 코드</param>
        /// <returns></returns>
        [HttpGet("{divCode}")]      // GET /api/divisions/{divCode}
        public async Task<IActionResult> GetDivisionAsync(string divCode)
        {
            using var conn = new MySqlConnection(connString);
            await conn.OpenAsync();

            string query =
                """
                SELECT div_code, div_name
                  FROM division
                 WHERE div_code = @divCode
                """;

            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@divCode", divCode);

            using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                Division division = new()
                {
                    DivCode = reader.GetString("div_code"),
                    DivName = reader.GetString("div_name")
                };

                return Ok(division);
            }

            return NotFound(new
            {
                message = "해당 도서 분류를 찾을 수 없습니다.",
                divCode
            });
        }

        /// <summary>
        /// 도서 분류 등록
        /// </summary>
        /// <param name="division">도서 분류 정보</param>
        /// <returns></returns>
        [HttpPost]  // POST /api/divisions
        public async Task<IActionResult> CreateDivision(Division division)
        {
            using var conn = new MySqlConnection(connString);
            await conn.OpenAsync();

            string query =
                """
                INSERT INTO division
                (
                    div_code,
                    div_name
                )
                VALUES
                (
                    @DivCode,
                    @DivName
                );
                """;

            using var cmd = new MySqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@DivCode", division.DivCode);
            cmd.Parameters.AddWithValue("@DivName", division.DivName);

            try
            {
                int result = await cmd.ExecuteNonQueryAsync();

                if (result == 0)
                {
                    return BadRequest("도서 분류 등록에 실패했습니다.");
                }

                return Ok(division);
            }
            catch (MySqlException ex)
            {
                return BadRequest(new
                {
                    message = "도서 분류 등록 중 오류가 발생했습니다.",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// 도서 분류 수정
        /// </summary>
        /// <param name="divCode">도서 분류 코드</param>
        /// <param name="division">도서 분류 정보</param>
        /// <returns></returns>
        [HttpPut("{divCode}")]  // PUT /api/divisions/{divCode}
        public async Task<IActionResult> UpdateDivision(string divCode, Division division)
        {
            using var conn = new MySqlConnection(connString);
            await conn.OpenAsync();

            string query =
                """
                UPDATE division
                   SET div_name = @DivName
                 WHERE div_code = @DivCode;
                """;

            using var cmd = new MySqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@DivName", division.DivName);
            cmd.Parameters.AddWithValue("@DivCode", divCode);

            int result = await cmd.ExecuteNonQueryAsync();

            if (result == 0)
            {
                return NotFound($"도서 분류 코드 {divCode}를 찾을 수 없습니다.");
            }

            return Ok("도서 분류가 수정되었습니다.");
        }

        /// <summary>
        /// 도서 분류 삭제
        /// </summary>
        /// <param name="divCode">도서 분류 코드</param>
        /// <returns></returns>
        [HttpDelete("{divCode}")]   // DELETE /api/divisions/{divCode}
        public async Task<IActionResult> DeleteDivision(string divCode)
        {
            using var conn = new MySqlConnection(connString);
            await conn.OpenAsync();

            string query =
                """
                DELETE FROM division
                 WHERE div_code = @DivCode;
                """;

            using var cmd = new MySqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@DivCode", divCode);

            try
            {
                int result = await cmd.ExecuteNonQueryAsync();

                if (result == 0)
                {
                    return NotFound($"도서 분류 코드 {divCode}를 찾을 수 없습니다.");
                }

                return Ok("도서 분류가 삭제되었습니다.");
            }
            catch (MySqlException ex)
            {
                return BadRequest(new
                {
                    message = "도서 분류 삭제 중 오류가 발생했습니다.",
                    error = ex.Message
                });
            }
        }
    }
}
