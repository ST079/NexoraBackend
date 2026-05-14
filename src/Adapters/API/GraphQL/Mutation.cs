namespace NexoraBackend.API.GraphQL;

// This class acts as the root for your Mutation type
public class Mutation
{
    // You can leave this empty, or add a dummy field if needed
    // But usually, adding[ExtendObjectType(typeof(Mutation))] to your 
    // UserMutation and RoleMutation classes is enough if you do the next step.

    public string Greet() => "Its Working";

}