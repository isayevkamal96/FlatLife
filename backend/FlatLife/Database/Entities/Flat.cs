using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using FlatLife.Database.Entities;
[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Postgres likes snake case better")]
public class Flat
{

    [Required]
    [Key]
    public int id { get; set; }

    [Required(AllowEmptyStrings = false)]
    public string flatName { get; set; } = "";

    public string flatShortName { get; set; } = "";



}