﻿using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Betalgo.Ranul.OpenAI.ObjectModels.RequestModels;

/// <summary>
///     The contents of the message.
///     Messages must be an array of message objects, where each object has a role (either “system”, “user”, or
///     “assistant”) and content (the content of the message) and an optional name
/// </summary>
public class ChatMessage
{
    public ChatMessage()
    {
    }

    /// <summary>
    /// </summary>
    /// <param name="role">The role of the author of this message. One of system, user, or assistant.</param>
    /// <param name="content">The contents of the message.</param>
    /// <param name="name">
    ///     The name of the author of this message. May contain a-z, A-Z, 0-9, and underscores, with a maximum
    ///     length of 64 characters.
    /// </param>
    /// <param name="toolCallId">The tool function call id generated by the model</param>
    /// <param name="toolCalls">The tool calls generated by the model.</param>
    public ChatMessage(string role, string content, string? name = null, IList<ToolCall>? toolCalls = null, string? toolCallId = null)
    {
        Role = role;
        Content = content;
        Name = name;
        ToolCalls = toolCalls;
        ToolCallId = toolCallId;
    }

    /// <summary>
    /// </summary>
    /// <param name="role">The role of the author of this message. One of system, user, or assistant.</param>
    /// <param name="contents">The list of the content messages.</param>
    /// <param name="name">
    ///     The name of the author of this message. May contain a-z, A-Z, 0-9, and underscores, with a maximum
    ///     length of 64 characters.
    /// </param>
    /// <param name="toolCallId">The tool function call id generated by the model</param>
    /// <param name="toolCalls">The tool calls generated by the model.</param>
    public ChatMessage(string role, IList<MessageContent> contents, string? name = null, IList<ToolCall>? toolCalls = null, string? toolCallId = null)
    {
        Role = role;
        Contents = contents;
        Name = name;
        ToolCalls = toolCalls;
        ToolCallId = toolCallId;
    }

    /// <summary>
    ///     The role of the author of this message. One of system, user, or assistant.
    /// </summary>
    [JsonPropertyName("role")]
    public string Role { get; set; }

    [JsonIgnore]
    public string? Content { get; set; }
    
    [JsonPropertyName("reasoning_content")]
    public string? ReasoningContent { get; set; }

    [JsonIgnore]
    public IList<MessageContent>? Contents { get; set; }

    /// <summary>
    ///     The contents of the message.
    /// </summary>
    [JsonPropertyName("content")]
    public object ContentCalculated
    {
        get
        {
            if (Content is not null && Contents is not null)
            {
                throw new ValidationException("Content and Contents can not be assigned at the same time. One of them must be null.");
            }

            if (Content is not null)
            {
                return Content;
            }

            return Contents!;
        }
        set => Content = value?.ToString();
    }

    /// <summary>
    ///     The name of the author of this message. May contain a-z, A-Z, 0-9, and underscores, with a maximum length of 64
    ///     characters.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    ///     Required for tool role messages.
    ///     Tool call that this message is responding to.
    /// </summary>
    [JsonPropertyName("tool_call_id")]
    public string? ToolCallId { get; set; }

    /// <summary>
    ///     Deprecated and replaced by tool_calls. The name and arguments of a function that should be called, as generated by
    ///     the model.
    /// </summary>
    [JsonPropertyName("function_call")]
    public FunctionCall? FunctionCall { get; set; }

    /// <summary>
    ///     The tool calls generated by the model, such as function calls.
    /// </summary>
    [JsonPropertyName("tool_calls")]
    public IList<ToolCall>? ToolCalls { get; set; }

    public static ChatMessage FromAssistant(string content, string? name = null, IList<ToolCall>? toolCalls = null)
    {
        return new(StaticValues.ChatMessageRoles.Assistant, content, name, toolCalls);
    }

    public static ChatMessage FromTool(string content, string toolCallId)
    {
        return new(StaticValues.ChatMessageRoles.Tool, content, toolCallId: toolCallId);
    }

    public static ChatMessage FromUser(string content, string? name = null)
    {
        return new(StaticValues.ChatMessageRoles.User, content, name);
    }

    public static ChatMessage FromSystem(string content, string? name = null)
    {
        return new(StaticValues.ChatMessageRoles.System, content, name);
    }

    public static ChatMessage FromUser(IList<MessageContent> contents)
    {
        return new(StaticValues.ChatMessageRoles.User, contents);
    }
}