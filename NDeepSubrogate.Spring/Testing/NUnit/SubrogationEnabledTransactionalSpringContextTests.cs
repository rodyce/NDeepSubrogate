﻿#region License

/*
 * Copyright 2017 Rodimiro Cerrato Espinal.
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *      http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#endregion


using NUnit.Framework;
using Spring.Testing.NUnit;

namespace NDeepSubrogate.Spring.Testing.NUnit
{
    public abstract class SubrogationEnabledTransactionalSpringContextTests : AbstractTransactionalSpringContextTests
    {
        private SpringDeepSubrogationScope _subrogationScope;

        [TestFixtureSetUp]
        protected void SubrogationInit()
        {
            var appContext = GetContext(ContextKey);
            _subrogationScope = new SpringDeepSubrogationScope(this, appContext);
        }

        [SetUp]
        protected void DoSubrogateContext()
        {
            Assert.NotNull(_subrogationScope);
            _subrogationScope.DeepSubrogate();
        }

        [TearDown]
        protected void DoRestoreContext()
        {
            _subrogationScope?.DeepRestore();
        }

        [TestFixtureTearDown]
        protected void SubrogationCleanup()
        {
            _subrogationScope = null;
        }
    }
}
