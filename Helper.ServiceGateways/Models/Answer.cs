namespace Helper.ServiceGateways.Models;

public class Answer
{
    public int AnswerId { get; set; }
    public int ResponseId { get; set; }
    public Response? Response { get; set; }
    public required string Text { get; set; }
}