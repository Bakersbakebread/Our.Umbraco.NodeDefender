using System.Collections.Generic;

namespace NodeDefender.Models;

public class DenyOptionsDto
{
    public string Type { get; set; }
    public string Message { get; set; }
    public List<DoctypeDto> Doctype { get; set; }
}