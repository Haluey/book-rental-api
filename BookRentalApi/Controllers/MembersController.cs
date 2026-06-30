using BookRentalApi.Models;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;

namespace BookRentalApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // localhost:port/api/members
    public class MembersController : ControllerBase
    {
        readonly string connString;

        public MembersController(IConfiguration configuration)
        {
            connString = configuration.GetConnectionString("BookRentalDbConnection")!;
        }

        /// <summary>
        /// 회원 목록 조회
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetMembersAsync()
        {
            List<Member> members = new();

            using var conn = new MySqlConnection(connString);
            await conn.OpenAsync();

            string query =
                """
                SELECT member_idx, member_name, levels, address, mobile, email
                  FROM members
                 ORDER BY member_idx DESC
                """;

            using var cmd = new MySqlCommand(query, conn);
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                Member member = new()
                {
                    MemberIdx = reader.GetInt32("member_idx"),
                    MemberName = reader.GetString("member_name"),
                    Levels = reader.GetString("levels"),
                    Address = reader.GetString("address"),
                    Mobile = reader.GetString("mobile"),
                    Email = reader.GetString("email")
                };

                members.Add(member);
            }

            return Ok(members);
        }

        /// <summary>
        /// 회원 단건 조회
        /// </summary>
        /// <param name="id">조회할 회원 번호</param>
        /// <returns></returns>
        [HttpGet("{id}")]   // GET /api/members/{id}
        public async Task<IActionResult> GetMemberAsync(int id)
        {
            using var conn = new MySqlConnection(connString);
            await conn.OpenAsync();

            string query =
                """
                SELECT member_idx, member_name, levels, address, mobile, email
                  FROM members
                 WHERE member_idx = @MemberIdx
                """;

            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@MemberIdx", id);

            using var reader = await cmd.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
            {
                return NotFound($"회원 번호 {id}를 찾을 수 없습니다.");
            }

            Member member = new()
            {
                MemberIdx = reader.GetInt32("member_idx"),
                MemberName = reader.GetString("member_name"),
                Levels = reader.GetString("levels"),
                Address = reader.GetString("address"),
                Mobile = reader.GetString("mobile"),
                Email = reader.GetString("email")
            };

            return Ok(member);
        }

        /// <summary>
        /// 회원 등록
        /// </summary>
        /// <param name="member">등록할 회원 정보</param>
        /// <returns></returns>
        [HttpPost]  // POST /api/members
        public async Task<IActionResult> CreateMember(MemberRequest member)
        {
            using var conn = new MySqlConnection(connString);
            await conn.OpenAsync();

            string query =
                """
                INSERT INTO members
                (
                    member_name,
                    levels,
                    address,
                    mobile,
                    email
                )
                VALUES
                (
                    @MemberName,
                    @Levels,
                    @Address,
                    @Mobile,
                    @Email
                );

                SELECT LAST_INSERT_ID();
                """;

            using var cmd = new MySqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@MemberName", member.MemberName);
            cmd.Parameters.AddWithValue("@Levels", member.Levels);
            cmd.Parameters.AddWithValue("@Address", member.Address);
            cmd.Parameters.AddWithValue("@Mobile", member.Mobile);
            cmd.Parameters.AddWithValue("@Email", member.Email);

            int newId = Convert.ToInt32(await cmd.ExecuteScalarAsync());

            return Ok(new
            {
                memberIdx = newId,
                member.MemberName,
                member.Levels,
                member.Address,
                member.Mobile,
                member.Email
            });
        }

        /// <summary>
        /// 회원 수정
        /// </summary>
        /// <param name="id">수정할 회원 번호</param>
        /// <param name="member">수정할 회원 정보</param>
        /// <returns></returns>
        [HttpPut("{id}")]   // PUT /api/members/{id}
        public async Task<IActionResult> UpdateMember(int id, MemberRequest member)
        {
            using var conn = new MySqlConnection(connString);
            await conn.OpenAsync();

            string query =
                """
                UPDATE members
                   SET member_name = @MemberName,
                       levels = @Levels,
                       address = @Address,
                       mobile = @Mobile,
                       email = @Email
                 WHERE member_idx = @MemberIdx;
                """;

            using var cmd = new MySqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@MemberName", member.MemberName);
            cmd.Parameters.AddWithValue("@Levels", member.Levels);
            cmd.Parameters.AddWithValue("@Address", member.Address);
            cmd.Parameters.AddWithValue("@Mobile", member.Mobile);
            cmd.Parameters.AddWithValue("@Email", member.Email);
            cmd.Parameters.AddWithValue("@MemberIdx", id);

            int result = await cmd.ExecuteNonQueryAsync();

            if (result == 0)
            {
                return NotFound($"회원 번호 {id}를 찾을 수 없습니다.");
            }

            return Ok("회원 정보가 수정되었습니다.");
        }

        /// <summary>
        /// 회원 삭제
        /// </summary>
        /// <param name="id">삭제할 회원 번호</param>
        /// <returns></returns>
        [HttpDelete("{id}")]    // DELETE /api/members/{id}
        public async Task<IActionResult> DeleteMember(int id)
        {
            using var conn = new MySqlConnection(connString);
            await conn.OpenAsync();

            string query =
                """
                DELETE FROM members
                 WHERE member_idx = @MemberIdx;
                """;

            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@MemberIdx", id);

            try
            {
                int result = await cmd.ExecuteNonQueryAsync();

                if (result == 0)
                {
                    return NotFound($"회원 번호 {id}를 찾을 수 없습니다.");
                }

                return Ok("회원이 삭제되었습니다.");
            }
            catch (MySqlException ex)
            {
                return BadRequest(new
                {
                    message = "회원 삭제 중 오류가 발생했습니다.",
                    error = ex.Message
                });
            }
        }
    }
}
