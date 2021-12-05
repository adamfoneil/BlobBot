using BlobBot.Shared.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace BlobBot.Shared.Models
{
    public enum Status
    {
        New,
        Processing,
        Succeeded,
        Failed
    }

    /// <summary>
    /// a database table inserted by Azure EventGrid whenever a blob is created
    /// </summary>
    [Table("BlobCreated", Schema = "eventgrid")]        
    public class BlobCreated : IBlobInfo
    {
        [Key]
        public long Id { get; set; }
        public string Container { get; set; }
        public string Name { get; set; }
        public DateTime DateCreated { get; set; }
        public long Length { get; set; }
        public string ContentType { get; set; }
        public Interfaces.Status Status { get; set; }
        public string ProcessedBy { get; set; }
        public DateTime? StatusDateTime { get; set; }
        public string ErrorMessage { get; set; }
    }

    /// <summary>
    /// inserted by Event Grid whenever a blob is deleted (assuming it was first added to BlobCreated)
    /// </summary>
    [Table("BlobDeleted", Schema = "eventgrid")]
    public class BlobDeleted : BlobCreated
    {
        /// <summary>
        /// BlobCreated.Id
        /// </summary>
        public long SourceId { get; set; }
        public DateTime DateDeleted { get; set; }
    }
}
