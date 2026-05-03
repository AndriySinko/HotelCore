// This file contains code for GetMasterProjectByIdQuery.
using MediatR;
using HotelCore.Application.MastersProjects.DTOs;

namespace HotelCore.Application.MastersProjects.Queries.GetMasterProjectById;

public record GetMasterProjectByIdQuery (Guid masterProjectId) : IRequest<MasterProjectDto>;
