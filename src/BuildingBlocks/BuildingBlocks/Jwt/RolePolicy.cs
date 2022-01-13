using System.Collections.Generic;

namespace BuildingBlocks.Jwt;

public class RolePolicy
{
    public string Name { get; set; }
    public IList<string> Roles { get; set; }
}
