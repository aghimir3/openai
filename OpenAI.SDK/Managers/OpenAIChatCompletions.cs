﻿using System.Runtime.CompilerServices;
using Betalgo.Ranul.OpenAI.Extensions;
using Betalgo.Ranul.OpenAI.Interfaces;
using Betalgo.Ranul.OpenAI.ObjectModels.RequestModels;
using Betalgo.Ranul.OpenAI.ObjectModels.ResponseModels;

namespace Betalgo.Ranul.OpenAI.Managers;

public partial class OpenAIService : IChatCompletionService
{
    /// <inheritdoc />
    public async Task<ChatCompletionCreateResponse> CreateCompletion(ChatCompletionCreateRequest chatCompletionCreateRequest, string? modelId = null, CancellationToken cancellationToken = default)
    {
        chatCompletionCreateRequest.ProcessModelId(modelId, _defaultModelId);
        return await _httpClient.PostAndReadAsAsync<ChatCompletionCreateResponse>(_endpointProvider.ChatCompletionCreate(), chatCompletionCreateRequest, cancellationToken);
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<ChatCompletionCreateResponse> CreateCompletionAsStream(ChatCompletionCreateRequest chatCompletionCreateRequest, string? modelId = null, bool justDataMode = true,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        // Mark the request as streaming
        chatCompletionCreateRequest.Stream = true;

        // Send the request to the CompletionCreate endpoint
        chatCompletionCreateRequest.ProcessModelId(modelId, _defaultModelId);

        using var response = _httpClient.PostAsStreamAsync(_endpointProvider.ChatCompletionCreate(), chatCompletionCreateRequest, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            yield return await response.HandleResponseContent<ChatCompletionCreateResponse>(cancellationToken);
            yield break;
        }

        await foreach (var baseResponse in response.AsStream<ChatCompletionCreateResponse>(cancellationToken: cancellationToken)) yield return baseResponse;
    }
}