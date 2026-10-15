using Npgsql;
using Microsoft.Extensions.Configuration;
using library.Models;

namespace library.Data;

public class MemberRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public MemberRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    private static Member ReadMember(NpgsqlDataReader reader)
    {
        return new Member
        {
            MemberId = reader.GetInt64(0),
            FullName = reader.GetString(1),
            Email = reader.IsDBNull(2) ? null : reader.GetString(2),
            MemberType = reader.GetString(3)
        };
    }

    public async Task<IEnumerable<Member>> GetAllAsync()
    {
        var members = new List<Member>();
        using var conn = await _dataSource.OpenConnectionAsync();
        using var cmd = new NpgsqlCommand("SELECT member_id, full_name, email, member_type FROM lending.member ORDER BY full_name;", conn);
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            members.Add(ReadMember(reader));
        }
        return members;
    }

    public async Task<Member?> GetByIdAsync(long id)
    {
        using var conn = await _dataSource.OpenConnectionAsync();
        using var cmd = new NpgsqlCommand("SELECT member_id, full_name, email, member_type FROM lending.member WHERE member_id = @id;", conn);
        cmd.Parameters.AddWithValue("id", id);
        using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return ReadMember(reader);
        }
        return null;
    }

    public async Task AddAsync(Member member)
    {
        using var conn = await _dataSource.OpenConnectionAsync();
        using var cmd = new NpgsqlCommand("INSERT INTO lending.member (full_name, email, member_type) VALUES (@fullName, @email, @memberType);", conn);
        cmd.Parameters.AddWithValue("fullName", member.FullName);
        cmd.Parameters.AddWithValue("email", (object?)member.Email ?? DBNull.Value);
        cmd.Parameters.AddWithValue("memberType", member.MemberType);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task UpdateAsync(Member member)
    {
        using var conn = await _dataSource.OpenConnectionAsync();
        using var cmd = new NpgsqlCommand("UPDATE lending.member SET full_name = @fullName, email = @email, member_type = @memberType WHERE member_id = @id;", conn);
        cmd.Parameters.AddWithValue("id", member.MemberId);
        cmd.Parameters.AddWithValue("fullName", member.FullName);
        cmd.Parameters.AddWithValue("email", (object?)member.Email ?? DBNull.Value);
        cmd.Parameters.AddWithValue("memberType", member.MemberType);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task DeleteAsync(long id)
    {
        using var conn = await _dataSource.OpenConnectionAsync();
        using var cmd = new NpgsqlCommand("DELETE FROM lending.member WHERE member_id = @id;", conn);
        cmd.Parameters.AddWithValue("id", id);
        await cmd.ExecuteNonQueryAsync();
    }
}