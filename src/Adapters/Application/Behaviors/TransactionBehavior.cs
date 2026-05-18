namespace NexoraBackend.Application.Behaviors;


using MediatR;
using NexoraBackend.Application.Interfaces.Repositories;


//Marker Interface
public interface ITransactionalCommand
{
    //It tells system that this command needs transaction safety.
    //it has no properties or methods, Its just a tag. When MediatR sees a command that implements this interface, 
    //it knows to wrap the execution of that command in a transaction using the TransactionBehavior.
}
 /*Why using marker interface? cause not everything needs transaction safety
 Like Query (egGetProducts) doesnot need transaction safety as only reads data*/


//Ensures either everything succeeds or everything fails, preventing data corruption and maintaining consistency.
// Ensures Atomicity
public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public TransactionBehavior(IUnitOfWork unitOfWork) 
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<TResponse> Handle(TRequest request,
        RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {   
        //If command doesn’t require transaction → just execute normally
        if (request is not ITransactionalCommand)
            return await next();


        //This opens the DB Transction.
        //now all db operations are temporary until we commit.
        await _unitOfWork.BeginTransactionAsync(ct);
        try
        {   
            // Runs the actual commnd handler.
            var response = await next();

            //If everything is successful → commit transaction, making all changes permanent in the database.
            await _unitOfWork.CommitTransactionAsync(ct);
            return response;
        }
        catch
        {
            //if anything fails, undo everything.
            await _unitOfWork.RollbackTransactionAsync(ct);
            throw;
        }
    }
}