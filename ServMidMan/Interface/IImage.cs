namespace ServMidMan.Interface
{
    public interface IImage
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public int ProductReferenceId { get; set; }
    }
}
