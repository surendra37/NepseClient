using System.Windows.Input;

namespace NepseClient.Libraries.NepalStockExchange.Responses
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class NoticeContent
    {
        public int Id { get; set; }
        public bool Deleted { get; set; }
        public int? DeletedBy { get; set; }
        public string DeletedOn { get; set; }
        public int? AddedBy { get; set; }
        public string AddedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public string ModifiedDate { get; set; }
        public int VersionId { get; set; }
        public string NoticeHeading { get; set; }
        public string NoticeBody { get; set; }
        public string NoticeExpiryDate { get; set; }
        public string NoticeFilePath { get; set; }
        public string NoticeFileFullPath { get; set; }
        public bool HasAttachment => !string.IsNullOrEmpty(NoticeFilePath);
        public ICommand OpenAttachmentCommand { get; set; }
    }

    public class NoticeReponse
    {
        public NoticeContent[] Content { get; set; }
        public Pageable Pageable { get; set; }
        public int TotalPages { get; set; }
        public int TotalElements { get; set; }
        public bool Last { get; set; }
        public int Number { get; set; }
        public int Size { get; set; }
        public int NumberOfElements { get; set; }
        public Sort Sort { get; set; }
        public bool First { get; set; }
        public bool Empty { get; set; }
    }
}
