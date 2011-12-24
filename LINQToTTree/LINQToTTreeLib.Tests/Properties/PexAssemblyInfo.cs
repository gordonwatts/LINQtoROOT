using System;
using System.Runtime.CompilerServices;
using LINQToTTreeLib;
using LINQToTTreeLib.Statements;
using LINQToTTreeLib.Utils;
// <copyright file="PexAssemblyInfo.cs" company="Microsoft">Copyright © Microsoft 2010</copyright>
using Microsoft.Pex.Framework.Coverage;
using Microsoft.Pex.Framework.Creatable;
using Microsoft.Pex.Framework.Instrumentation;
using Microsoft.Pex.Framework.Moles;
using Microsoft.Pex.Framework.Settings;
using Microsoft.Pex.Framework.Suppression;
using Microsoft.Pex.Framework.Using;
using Microsoft.Pex.Framework.Validation;
using Microsoft.Pex.Linq;
using Remotion.Linq.Clauses.ResultOperators;

// Microsoft.Pex.Framework.Settings
[assembly: PexAssemblySettings(TestFramework = "VisualStudioUnitTest")]

// Microsoft.Pex.Framework.Instrumentation
[assembly: PexAssemblyUnderTest("LINQToTTreeLib")]
[assembly: PexInstrumentAssembly("System.Core")]

// Microsoft.Pex.Framework.Creatable
[assembly: PexCreatableFactoryForDelegates]

// Microsoft.Pex.Framework.Validation
[assembly: PexAllowedContractRequiresFailureAtTypeUnderTestSurface]
[assembly: PexAllowedXmlDocumentedException]

// Microsoft.Pex.Framework.Coverage
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "System.Core")]

// Microsoft.Pex.Framework.Moles
[assembly: PexAssumeContractEnsuresFailureAtBehavedSurface]
[assembly: PexChooseAsBehavedCurrentBehavior]

// Microsoft.Pex.Linq
[assembly: PexLinqPackage]

[assembly: PexUseType(typeof(GC), "System.RuntimeType")]
[assembly: PexUseType(typeof(CountResultOperator))]
[assembly: PexUseType(typeof(CastResultOperator))]
[assembly: PexUseType(typeof(StatementIncrementInteger))]
[assembly: PexInstrumentAssembly("Remotion.Data.Linq")]
[assembly: PexUseType(typeof(GeneratedCode))]
[assembly: PexSuppressUninstrumentedMethodFromType(typeof(ConditionalWeakTable<,>))]
[assembly: PexSuppressStaticFieldStore("System.ComponentModel.Composition.ContractNameServices", "typeIdentityCache")]
[assembly: PexSuppressStaticFieldStore("System.Linq.EmptyEnumerable`1", "instance")]
[assembly: PexSuppressStaticFieldStore("LINQToTTreeLib.Tests.MEFUtilities", "_batch")]
[assembly: PexSuppressUninstrumentedMethodFromType(typeof(Buffer))]
[assembly: PexSuppressStaticFieldStore("LINQToTTreeLib.Tests.DummyQueryExectuor", "GlobalInitalized")]
[assembly: PexSuppressStaticFieldStore(typeof(TypeUtils), "_variableNameCounter")]
[assembly: PexSuppressStaticFieldStore("LINQToTTreeLib.Tests.DummyQueryExectuor", "<FinalResult>k__BackingField")]
