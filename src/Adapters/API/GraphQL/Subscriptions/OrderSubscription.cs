using HotChocolate.Authorization;
using NexoraBackend.Application.DTOs;

namespace NexoraBackend.API.GraphQL.Subscriptions;

[SubscriptionType] //tells hotchoclate, This class contains GraphQL subscriptions
//When something happens to an order, backend automatically pushes updates to frontend without frontend asking again.
public class OrderSubscriptions
{
    [Authorize] // only authenticated users can subscribe to order updates
    [Subscribe] //This tells GraphQL: This method is a subscription endpoint, Without this → it won’t work.
    /*
    This creates a dynamic topic like:
        123-order-updated
        456-order-updated
        789-order-updated
    Each user gets their own private topic.
    */
    [Topic("{userId}:order-updated")]

    public OrderDto OnOrderUpdated(
        Guid userId,
        [EventMessage] OrderDto order) => order;


    //Notify Admin whenever new order is placed
    [Authorize(Roles = new[] { "Admin" })]
    [Subscribe]
    [Topic("new-order")]
    public OrderDto OnNewOrder([EventMessage] OrderDto order) => order;
}