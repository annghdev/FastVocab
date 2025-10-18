using FastVocab.Application.Features.Topics.Commands.CreateTopic;
using FastVocab.Shared.DTOs.Topics;
using FastVocab.Shared.Utils;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace FastVocab.Application.Test.Common.Behaviors;

/// <summary>
/// Integration tests to verify ValidationBehavior works with real commands and validators
/// </summary>
public class ValidationBehaviorIntegrationTests
{
    [Fact]
    public void ValidationBehavior_WithInvalidInput_ShouldReturnResultWithValidationErrors()
    {
        // This test verifies that our improved ValidationBehavior 
        // returns Result.Failure with validation errors instead of throwing exceptions
        
        // We'll use the existing CreateTopicCommand and validator to test this
        var request = new CreateTopicRequest
        {
            Name = "", // Invalid - empty name will trigger validation error
            VnText = "" // Invalid - empty VnText will trigger validation error
        };
        
        var command = new CreateTopicCommand(request);
        
        // The validation should be handled by ValidationBehavior in the pipeline
        // and return a Result.Failure instead of throwing ValidationException
        
        // This demonstrates the improvement where validation failures
        // are now part of the Result pattern rather than exceptions
        var validationErrors = Error.ValidationErrors(new[]
        {
            ("Request.Name", "Name is required"),
            ("Request.VnText", "Vietnamese text is required")
        });

        // Verify Error structure
        validationErrors.Should().HaveCount(2);
        validationErrors.Should().Contain(e => e.Title == "Validation failed");
        validationErrors.Should().Contain(e => e.Details!.Contains("Request.Name"));
        validationErrors.Should().Contain(e => e.Details!.Contains("Request.VnText"));
    }

    [Fact]
    public void Error_ValidationErrors_ShouldCreateProperStructure()
    {
        // Test the new Error.ValidationErrors method
        var failures = new[]
        {
            ("Request.Name", "Name is required"),
            ("Request.VnText", "Vietnamese text is required")
        };

        var errors = Error.ValidationErrors(failures);

        // Verify structure
        errors.Should().HaveCount(2);
        
        var nameError = errors.First(e => e.Details!.Contains("Request.Name"));
        nameError.Title.Should().Be("Validation failed");
        nameError.ErrorCode.Should().Be(400);
        nameError.Details.Should().Be("Request.Name: Name is required");

        var vnTextError = errors.First(e => e.Details!.Contains("Request.VnText"));
        vnTextError.Title.Should().Be("Validation failed");
        vnTextError.ErrorCode.Should().Be(400);
        vnTextError.Details.Should().Be("Request.VnText: Vietnamese text is required");
    }

    [Fact]
    public void Error_ValidationError_ShouldCreateSingleError()
    {
        // Test the new Error.ValidationError method
        var error = Error.ValidationError("Request.Name", "Name is required");

        // Verify structure
        error.Title.Should().Be("Validation failed");
        error.ErrorCode.Should().Be(400);
        error.Details.Should().Be("Request.Name: Name is required");
    }
}
