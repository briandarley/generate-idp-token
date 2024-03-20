namespace generate_idp_token.Interfaces;

public interface IWorkerTask
{
    Task ExecuteAsync(CancellationToken cancellationToken);
}