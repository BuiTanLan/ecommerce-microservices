# ECommerce Microservices


## The Domain and Bounded Context (Service Boundary)

`ECommerce Microservices` is a simple e-commerce application that has the basic business scenario for online purchasing with some dedicated services. There are six possible `Bounded context` or `Service` for above business:

- `Identity Service`: the Identity Service uses to authenticate and authorize users through a token. Also, this service is responsible for creating users and their corresponding roles and permission with using [.Net Core Identity](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity) and Jwt authentication and authorization. I will add also [Identity Server](https://github.com/DuendeSoftware/IdentityServer) in future for this service. Each of `Administrator`, `Customer` and `Supplier` are a `User`, actually a `IdentityUser`. To be a User, User Registration is required. Each User is assigned one or more User Role.
Each User Role has set of Permissions. A Permission defines whether User can invoke a particular action or not.

- `Catalog Service`: The Catalog Service presents the ability to add items to our store, It can be electronics, foods, books or anything else. Items can be grouped into categories and catalogs. A catalog is defined as a list of items that a company showcases online. the catalog is a collection of items, which can be grouped into categories. An item can be assigned to only one category or be direct child of a catalog without any category.
Buyer can browse the products list with supported filtering and sorting by product name and price. customer can see the detail of the product on the product list and in the detail page, can see a name, description, available product in the inventory,...

- `Customers Service`: This service is responsible for managing our customers information, track the activities and subscribing to get notification for out of stock products

- `Order Service`: The Orders Service main purpose is to store order details and manage orders created by users on client side. This service is not designed to be a full order processing system like ERP but serves as storage for customer orders details and can be synchronized with different external processing systems.
Some of this service responsibilities are `Saving orders`, `Saving order drafts`, `Ability to view and manage fulfillment, packages`, `Change discounts`

- `Payment Service`: The payment service is responsible for payment process of our customer with different payment process and managing and tracking our payment history

- `Shipping Service`: The Shipping Service provides the ability to extend shipping provider list with custom providers and also provides an interface and API for managing these shipping providers.
Some of shipping service capabilities are `Register Shipping methods`, `Edit Shipping method`, `Shipment details`, `Shipping settings`

## Prerequisites

1. Install git - [https://git-scm.com/downloads](https://git-scm.com/downloads).
2. Install .NET Core 6.0 - [https://dotnet.microsoft.com/download/dotnet/6.0](https://dotnet.microsoft.com/download/dotnet/6.0).
3. Install Visual Studio 2022, Rider or VSCode.
4. Install docker - [https://docs.docker.com/docker-for-windows/install/](https://docs.docker.com/docker-for-windows/install/).
5. Make sure that you have ~10GB disk space.
7. Clone Project [https://github.com/mehdihadeli/e-commerce-microservices](https://github.com/mehdihadeli/e-commerce-microservices), make sure that's compiling
8. Open `ECommerce.sln` solution.
9. Docker useful commands

    - `docker-compose up` - start dockers
    - `docker-compose kill` - to stop running dockers.
    - `docker-compose down -v` - to clean stopped dockers.
    - `docker ps` - for showing running dockers
    - `docker ps -a` - to show all dockers (also stopped)

10. Go to [deployments/docker-compose](./deployments/docker-compose) and run: `docker-compose up`.
11. Wait until all dockers got are downloaded and running.
12. You should automatically get:
    - Postgres running
    - RabbitMQ running
    - MongoDB running
    - Microservies Api Gateway, Available at: http://localhost:8000

## Application Structure

In this application I used a [mediator pattern](https://dotnetcoretutorials.com/2019/04/30/the-mediator-pattern-in-net-core-part-1-whats-a-mediator/) with using [MediatR](https://github.com/jbogard/MediatR) library in my controllers for a clean and [thin controller](https://codeopinion.com/thin-controllers-cqrs-mediatr/), also instead of using and injecting a `application service` class in our controller, we just inject a mediator class, because after some times our controller will depends to different services and this breaks single responsibility principle.

Mediator mediate between objects and prevent direct coupling between objects, and objects could communicate each other with sending some message through mediator as a gateway. Here We use mediator pattern to manage the delivery of messages to handlers. For example in our controllers we create a command and send it to mediator and mediator will route our command to a specific command handler in application layer.

One of the advantages behind the [mediator pattern](https://lostechies.com/jimmybogard/2014/09/09/tackling-cross-cutting-concerns-with-a-mediator-pipeline/) is that it allows us to define some pipelines of activities for requests on top of our mediator for doing some cross cutting concerns that brings [Single Responsibility Principle](https://en.wikipedia.org/wiki/Single_responsibility_principle) and [Don't Repeat Yourself principles](https://en.wikipedia.org/wiki/Don%27t_repeat_yourself) in our application.

For implementing these pipelines in our mediator to handle cross-cutting concerns we use MediatR libraries and its [Pipeline Behaviors](https://github.com/jbogard/MediatR/wiki/Behaviors) or we can create some [MediatR Decorators](https://lostechies.com/jimmybogard/2014/09/09/tackling-cross-cutting-concerns-with-a-mediator-pipeline/) as our pipelines.

Also in this project I used [vertical slice architecture](https://jimmybogard.com/vertical-slice-architecture/) or [Restructuring to a Vertical Slice Architecture](https://codeopinion.com/restructuring-to-a-vertical-slice-architecture/) also I used [feature folder structure](http://www.kamilgrzybek.com/design/feature-folders/) in this project.

- We treat each request as a distinct use case or slice, encapsulating and grouping all concerns from front-end to back.
- When We adding or changing a feature in an application in n-tire architecture, we are typically touching many different "layers" in an application. we are changing the user interface, adding fields to models, modifying validation, and so on. Instead of coupling across a layer, we couple vertically along a slice and each change affects only one slice.
- We `Minimize coupling` `between slices`, and `maximize coupling` `in a slice`.
- With this approach, each of our vertical slices can decide for itself how to best fulfill the request. New features only add code, we're not changing shared code and worrying about side effects. For implementing vertical slice architecture using cqrs pattern is a good match.

![](./assets/Vertical-Slice-Architecture.jpg)

![](./assets/vsa2.png)

Also here I used [CQRS](https://www.eventstore.com/cqrs-pattern) for decompose my features to very small parts that makes our application:

- maximize performance, scalability and simplicity.
- adding new feature to this mechanism is very easy without any breaking change in other part of our codes. New features only add code, we're not changing shared code and worrying about side effects.
- easy to maintain and any changes only affect on one command or query (or a slice) and avoid any breaking changes on other parts
- it gives us better separation of concerns and cross cutting concern (with help of mediatr behavior pipelines) in our code instead of a big service class for doing a lot of things.

With using CQRS, our code will be more aligned with [SOLID principles](https://en.wikipedia.org/wiki/SOLID), especially with:

- [Single Responsibility](https://en.wikipedia.org/wiki/Single-responsibility_principle) rule - because logic responsible for a given operation is enclosed in its own type.
- [Open-Closed](https://en.wikipedia.org/wiki/Open%E2%80%93closed_principle) rule - because to add new operation you don’t need to edit any of the existing types, instead you need to add a new file with a new type representing that operation.

We cut each business functionality into some vertical slices, and inner each of this slices we have [technical folders structure](http://www.kamilgrzybek.com/design/feature-folders/) specific to that feature (command, handlers, infrastructure, repository, controllers, ...). In Our CQRS pattern each command/query handler is a separate slice. This is where you can reduce coupling between layers. Each handler can be a separated code unit, even copy/pasted. Thanks to that, we can tune down the specific method to not follow general conventions (e.g. use custom SQL query or even different storage). In a traditional layered architecture, when we change the core generic mechanism in one layer, it can impact all methods.

For checking `validation rules` we use two type of validation:
- [Data Validation](http://www.kamilgrzybek.com/design/rest-api-data-validation/): Data validation verify data items which are coming to our application from external sources and check if theirs values are acceptable but Business rules validation is a more broad concept and more close to how business works and behaves. So it is mainly focused on behavior For implementing data validation I used [FluentValidation](https://github.com/FluentValidation/FluentValidation) library for cleaner validation also better separation of concern in my handlers for preventing mixing validation logic with orchestration logic in my handlers.
- [Business Rules validation](http://www.kamilgrzybek.com/design/domain-model-validation/): I explicitly check all of the our business rules, inner my handlers and I will throw a customized exception based on the error that these errors should inherits from [AppException](./src/BuildingBlocks/BulidingBlocks/Exception/AppException.cs) class, because of these exceptions, occurs in application layer and we catch this exceptions in api layer with using [Hellang.Middleware.ProblemDetails](https://www.nuget.org/packages/Hellang.Middleware.ProblemDetails/) middleware and pass a correct status code to client.
