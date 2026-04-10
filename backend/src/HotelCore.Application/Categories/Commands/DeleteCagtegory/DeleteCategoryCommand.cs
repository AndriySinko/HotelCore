// This file contains code for DeleteCategoryCommand.
using MediatR;

namespace HotelCore.Application.Categories.Commands.DeleteCagtegory;

public record DeleteCategoryCommand(Guid Id) : IRequest;