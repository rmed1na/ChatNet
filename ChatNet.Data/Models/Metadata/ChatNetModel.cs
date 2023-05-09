namespace ChatNet.Data.Models.Metadata
{
    public class ChatNetModel
    {
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public ChatNetModel()
        {
            CreatedDate = DateTime.Now;
        }

        public void SetUpdated()
            => UpdatedDate = DateTime.Now;
    }
}