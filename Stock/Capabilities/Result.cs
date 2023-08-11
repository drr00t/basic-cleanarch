// Copyright (C) 2022  Road to Agility
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Collections.Immutable;
using DFlow.Validation;

namespace Stock.Capabilities;

public class Result<TSucceded, TFailed>
{
    public bool IsSucceded { get; set; } 
    public TSucceded Succeded { get; set; } 
    public TFailed Failed { get; set; }
}

public sealed class Succeded<TSucceded>: Result<TSucceded, IReadOnlyList<Failure>>
{
    public static Result<TSucceded, IReadOnlyList<Failure>> SucceedFor(TSucceded value) =>
        new Result<TSucceded, IReadOnlyList<Failure>>{
            IsSucceded = true,
            Succeded = value, 
            Failed = ImmutableList<Failure>.Empty
        };
}

public sealed class Failed<TFailure> : Result<TFailure, IReadOnlyList<Failure>>
{
    public static Result<TFailure, IReadOnlyList<Failure>> FailedFor(IReadOnlyList<Failure> failures) =>
        new Result<TFailure, IReadOnlyList<Failure>>
        {
            IsSucceded = false,
            Failed = failures
        };
}