namespace NotForgottenTwo.Services
{
    public class UploadedImage
    {
        public string ContentType { get; set; }
        public byte[] Data { get; set; }
        public string Name { get; set; }
        public string Url { get;  set; }
    }
}