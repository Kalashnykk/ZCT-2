namespace server.Models
{
    public class tUploadHistory
    {
        public int ID { get; set; }  // Auto-generated, Primary Key
        public required string Image_url { get; set; }  // Image URL field
        public required string Extracted_text { get; set; }  // Extracted Text
        public int? Result { get; set; }  // Integer field
        public DateTime DataTime { get; set; }  // Date/Time field
    }
}
