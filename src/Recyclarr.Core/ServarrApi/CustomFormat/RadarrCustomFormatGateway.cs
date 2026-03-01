using System.Globalization;
using Recyclarr.ResourceProviders.Domain;
using Recyclarr.Servarr.CustomFormat;
using Recyclarr.TrashGuide.CustomFormat;
using RadarrApi = Recyclarr.Api.Radarr;

namespace Recyclarr.ServarrApi.CustomFormat;

internal class RadarrCustomFormatGateway(RadarrApi.ICustomFormatApi api) : ICustomFormatService
{
    public async Task<IReadOnlyList<CustomFormatResource>> GetCustomFormats(CancellationToken ct)
    {
        var result = await api.CustomformatGet(ct);
        return result.Select(ToDomain).ToList();
    }

    public async Task<CustomFormatResource?> CreateCustomFormat(
        CustomFormatResource cf,
        CancellationToken ct
    )
    {
        var result = await api.CustomformatPost(FromDomain(cf), ct);
        return ToDomain(result);
    }

    public async Task UpdateCustomFormat(CustomFormatResource cf, CancellationToken ct)
    {
        await api.CustomformatPut(cf.Id.ToString(CultureInfo.InvariantCulture), FromDomain(cf), ct);
    }

    public async Task DeleteCustomFormat(int customFormatId, CancellationToken ct)
    {
        await api.CustomformatDelete(customFormatId, ct);
    }

    private static CustomFormatResource ToDomain(RadarrApi.CustomFormatResource dto)
    {
        return new CustomFormatResource
        {
            Id = dto.Id,
            Name = dto.Name ?? "",
            IncludeCustomFormatWhenRenaming = dto.IncludeCustomFormatWhenRenaming ?? false,
            Specifications = dto.Specifications?.Select(SpecToDomain).ToList() ?? [],
        };
    }

    private static CustomFormatSpecificationData SpecToDomain(
        RadarrApi.CustomFormatSpecificationSchema dto
    )
    {
        return new CustomFormatSpecificationData
        {
            Name = dto.Name ?? "",
            Implementation = dto.Implementation ?? "",
            Negate = dto.Negate,
            Required = dto.Required,
            Fields = dto.Fields?.Select(FieldToDomain).ToList() ?? [],
        };
    }

    private static CustomFormatFieldData FieldToDomain(RadarrApi.Field dto)
    {
        return new CustomFormatFieldData { Name = dto.Name ?? "", Value = dto.Value };
    }

    private static RadarrApi.CustomFormatResource FromDomain(CustomFormatResource domain)
    {
        return new RadarrApi.CustomFormatResource
        {
            Id = domain.Id,
            Name = domain.Name,
            IncludeCustomFormatWhenRenaming = domain.IncludeCustomFormatWhenRenaming,
            Specifications = domain.Specifications.Select(SpecFromDomain).ToList(),
        };
    }

    private static RadarrApi.CustomFormatSpecificationSchema SpecFromDomain(
        CustomFormatSpecificationData domain
    )
    {
        return new RadarrApi.CustomFormatSpecificationSchema
        {
            Name = domain.Name,
            Implementation = domain.Implementation,
            Negate = domain.Negate,
            Required = domain.Required,
            Fields = domain.Fields.Select(FieldFromDomain).ToList(),
        };
    }

    private static RadarrApi.Field FieldFromDomain(CustomFormatFieldData domain)
    {
        // non-null: field values are always populated from guide JSON
        return new RadarrApi.Field { Name = domain.Name, Value = domain.Value! };
    }
}
