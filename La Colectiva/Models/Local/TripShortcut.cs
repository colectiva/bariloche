using La_Colectiva.Models.Classic;
using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace La_Colectiva.Models.Local
{

    [Table("TripShortcuts")]
    public class TripShortcutDto
    {
        [PrimaryKey]
        public string Id { get; set; }

        [ForeignKey(typeof(DockDto))]
        public int FromId { get; set; }
        [ManyToOne]
        public DockDto From { get; set; }

        [ForeignKey(typeof(DockDto))]
        public int ToId { get; set; }
        [ManyToOne]
        public DockDto To { get; set; }
        
        public DateTime CreatedAt { get; set; }

        public TripShortcutDto() { }

        public TripShortcutDto(int fromId, int toId) : this()
        {
            FromId = fromId;
            ToId = toId;
            Id = BuildId(fromId, toId);
            CreatedAt = DateTime.UtcNow;
        }

        public static string BuildId(int fromId, int toId) => $"{fromId}-{toId}";
    }

    class TripShortcut
    {

        public string Id { get; set; }
        public Dock From { get; set; }
        public Dock To { get; set; }
        public DateTime CreatedAt { get; set; }

        public TripShortcut() { }

        public TripShortcut(TripShortcutDto dto) : this()
        {
            if(dto != null)
            {
                this.Id = dto.Id;
                if (dto.From != null) this.From = Dock.Create(dto.From);
                else if (dto.FromId > 0) this.From = new Dock() { Id = dto.FromId };
                if (dto.To != null) this.To = Dock.Create(dto.To);
                else if (dto.ToId > 0) this.To = new Dock() { Id = dto.ToId };
                this.CreatedAt = dto.CreatedAt;
            }
        }
        
        public static TripShortcut Mapper(TripShortcutDto dto)
        {
            return new TripShortcut(dto);
        }

    }
}
