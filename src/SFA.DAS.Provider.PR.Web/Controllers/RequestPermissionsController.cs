using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Encoding;
using SFA.DAS.Provider.PR.Domain.Interfaces;
using SFA.DAS.Provider.PR.Domain.OuterApi.Requests.Commands;
using SFA.DAS.Provider.PR.Domain.OuterApi.Responses;
using SFA.DAS.Provider.PR.Web.Authorization;
using SFA.DAS.Provider.PR.Web.Infrastructure;
using SFA.DAS.Provider.PR.Web.Models;
using SFA.DAS.Provider.PR.Web.Models.AddEmployer;
using SFA.DAS.Provider.PR.Web.Services;

namespace SFA.DAS.Provider.PR.Web.Controllers;

[Authorize(Policy = nameof(PolicyNames.HasProviderAccount))]
[Route("{ukprn:int}/employers/{accountlegalentityid}/requestpermissions", Name = RouteNames.RequestPermissions)]
public class RequestPermissionsController(IOuterApiClient _outerApiclient, IEncodingService encodingService, IValidator<RequestPermissionsSubmitModel> _validator) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index([FromRoute] int ukprn, [FromRoute] string accountLegalEntityId, CancellationToken cancellationToken)
    {
        RequestPermissionsViewModel model = await CreateRequestPermissionsViewModel(ukprn, accountLegalEntityId, cancellationToken);

        model.BackLink = Url.RouteUrl(RouteNames.EmployerDetails, new { ukprn, accountLegalEntityId })!;

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Index([FromRoute] int ukprn, [FromRoute] string accountLegalEntityId, RequestPermissionsSubmitModel requestPermissionsSubmitModel,CancellationToken cancellationToken)
    {
        var accountLegalEntityIdDecoded = encodingService.Decode(accountLegalEntityId, EncodingType.PublicAccountLegalEntityId);

        GetProviderRelationshipResponse response = await _outerApiclient.GetProviderRelationship(ukprn, accountLegalEntityIdDecoded, cancellationToken);

        PermissionDescriptionsViewModel existingPermissions = OperationsMappingService.MapOperationsToDescriptions(response.Operations.ToList());
        requestPermissionsSubmitModel.ExistingPermissionToRecruit = existingPermissions.PermissionToRecruit!;
        requestPermissionsSubmitModel.ExistingPermissionToAddCohorts = existingPermissions.PermissionToAddCohorts!;

        var result = _validator.Validate(requestPermissionsSubmitModel);

        if (!IsModelValid(requestPermissionsSubmitModel))
        {
            var model = await CreateRequestPermissionsViewModel(ukprn, accountLegalEntityId, cancellationToken);
            return View(model);
        }

        await _outerApiclient.AddRequest(new AddAccountRequestCommand()
        {
            Ukprn = ukprn,
            AccountLegalEntityId = accountLegalEntityIdDecoded,
            RequestedBy = "",
            Operations = OperationsMappingService.MapDescriptionsToOperations(requestPermissionsSubmitModel),
            AccountId = response.AccountId
        }, cancellationToken);

        return;
        ////RedirectToAction("Index", new { Ukprn, AccountLegalEntityId });
    }

    private async Task<RequestPermissionsViewModel> CreateRequestPermissionsViewModel(long ukprn, string accountLegalEntityId, CancellationToken cancellationToken)
    {
        var accountLegalEntityIdDecoded = encodingService.Decode(accountLegalEntityId, EncodingType.PublicAccountLegalEntityId);

        GetProviderRelationshipResponse response = await _outerApiclient.GetProviderRelationship(ukprn, accountLegalEntityIdDecoded, cancellationToken);

        return (RequestPermissionsViewModel)response;
    }

    private bool IsModelValid(RequestPermissionsSubmitModel requestPermissionsSubmitModel)
    {
        var result = _validator.Validate(requestPermissionsSubmitModel);
        if (!result.IsValid)
        {
            result.AddToModelState(ModelState);
            return false;
        }
        return true;
    }
}
