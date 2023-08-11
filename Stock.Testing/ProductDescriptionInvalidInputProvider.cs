// Copyright (C) 2022  Road to Agility
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Collections;
using Stock.Domain;

namespace Stock.Testing;

public class ValidProductInputData
{
    public static IEnumerable<object> Numbers()
    {
        yield return ProductId.From(Guid.NewGuid());
        yield return ProductDescription.From("my description");
        yield return 3;
        yield return 4;
        yield return 5;
    }
}
