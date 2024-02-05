namespace ServMidMan.Interface
{
    public interface IImage
    {
        public int Id { get; set; }
        public byte[] ImageData { get; set; }
        public string FileName { get; set; }
        public string ProductReferenceId { get; set; }
    }
}
