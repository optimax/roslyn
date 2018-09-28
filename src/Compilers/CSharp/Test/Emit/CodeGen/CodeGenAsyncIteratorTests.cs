﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Text;
using Microsoft.CodeAnalysis.CSharp.Test.Utilities;
using Microsoft.CodeAnalysis.Test.Utilities;
using Xunit;

namespace Microsoft.CodeAnalysis.CSharp.UnitTests.CodeGen
{
    using Roslyn.Test.Utilities;
    using static Instruction;
    internal enum Instruction
    {
        Write,
        Yield,
        AwaitSlow,
        AwaitFast,
        YieldBreak
    }

    [CompilerTrait(CompilerFeature.AsyncStreams)]
    public class CodeGenAsyncIteratorTests : EmitMetadataTestBase
    {
        // PROTOTYPE(async-streams)
        // Test missing remaining types/members once BCL APIs are finalized (MRVTSL, IStrongBox, IValueTaskSource)
        // test missing AsyncTaskMethodBuilder<T> or missing members Create(), Task, ...
        // Test with yield or await in try/catch/finally
        // More tests with exception thrown
        // There is a case in GetIteratorElementType with IsDirectlyInIterator that relates to speculation, needs testing
        // yield break disallowed in finally and top-level script (see BindYieldBreakStatement); same for yield return (see BindYieldReturnStatement)
        // binding for yield return (BindYieldReturnStatement) validates escape rules, needs testing
        // test yield in async lambda (still error)
        // test exception handling (should capture and return the exception via the promise)
        // test IAsyncEnumerable<U> M<U>() ...
        // test with IAsyncEnumerable<dynamic>
        // other tests with dynamic?
        // test should cover both case with AwaitOnCompleted and AwaitUnsafeOnCompleted
        // test `async IAsyncEnumerable<int> M() { return TaskLike(); }`
        // Can we avoid making IAsyncEnumerable<T> special from the start? Making mark it with an attribute like we did for task-like?
        // Do some manual validation on debugging scenarios, including with exceptions (thrown after yield and after await).

        private void VerifyMissingMember(WellKnownMember member, params DiagnosticDescription[] expected)
        {
            var lib = CreateCompilationWithTasksExtensions(s_common);
            var lib_ref = lib.EmitToImageReference();

            string source = @"
using System.Threading.Tasks;
class C
{
    async System.Collections.Generic.IAsyncEnumerable<int> M() { await Task.CompletedTask; yield return 3; }
}
";
                var comp = CreateCompilationWithTasksExtensions(source, references: new[] { lib_ref });
                comp.MakeMemberMissing(member);
                comp.VerifyEmitDiagnostics(expected);
        }

        private void VerifyMissingType(WellKnownType type, params DiagnosticDescription[] expected)
        {
            var lib = CreateCompilationWithTasksExtensions(s_common);
            var lib_ref = lib.EmitToImageReference();

            string source = @"
using System.Threading.Tasks;
class C
{
    async System.Collections.Generic.IAsyncEnumerable<int> M() { await Task.CompletedTask; yield return 3; }
}
";
                var comp = CreateCompilationWithTasksExtensions(source, references: new[] { lib_ref });
                comp.MakeTypeMissing(type);
                comp.VerifyEmitDiagnostics(expected);
        }

        [Fact]
        public void MissingTypeAndMembers_ManualResetValueTaskSourceLogic()
        {
            VerifyMissingMember(WellKnownMember.System_Threading_Tasks_ManualResetValueTaskSourceLogic_T__ctor,
                // (5,64): error CS0656: Missing compiler required member 'System.Threading.Tasks.ManualResetValueTaskSourceLogic`1..ctor'
                //     async System.Collections.Generic.IAsyncEnumerable<int> M() { await Task.CompletedTask; yield return 3; }
                Diagnostic(ErrorCode.ERR_MissingPredefinedMember, "{ await Task.CompletedTask; yield return 3; }").WithArguments("System.Threading.Tasks.ManualResetValueTaskSourceLogic`1", ".ctor").WithLocation(5, 64)
                );

            VerifyMissingMember(WellKnownMember.System_Threading_Tasks_ManualResetValueTaskSourceLogic_T__GetResult,
                // (5,64): error CS0656: Missing compiler required member 'System.Threading.Tasks.ManualResetValueTaskSourceLogic`1.GetResult'
                //     async System.Collections.Generic.IAsyncEnumerable<int> M() { await Task.CompletedTask; yield return 3; }
                Diagnostic(ErrorCode.ERR_MissingPredefinedMember, "{ await Task.CompletedTask; yield return 3; }").WithArguments("System.Threading.Tasks.ManualResetValueTaskSourceLogic`1", "GetResult").WithLocation(5, 64)
                );

            VerifyMissingMember(WellKnownMember.System_Threading_Tasks_ManualResetValueTaskSourceLogic_T__GetStatus,
                // (5,64): error CS0656: Missing compiler required member 'System.Threading.Tasks.ManualResetValueTaskSourceLogic`1.GetStatus'
                //     async System.Collections.Generic.IAsyncEnumerable<int> M() { await Task.CompletedTask; yield return 3; }
                Diagnostic(ErrorCode.ERR_MissingPredefinedMember, "{ await Task.CompletedTask; yield return 3; }").WithArguments("System.Threading.Tasks.ManualResetValueTaskSourceLogic`1", "GetStatus").WithLocation(5, 64)
                );

            VerifyMissingMember(WellKnownMember.System_Threading_Tasks_ManualResetValueTaskSourceLogic_T__get_Version,
                // (5,64): error CS0656: Missing compiler required member 'System.Threading.Tasks.ManualResetValueTaskSourceLogic`1.get_Version'
                //     async System.Collections.Generic.IAsyncEnumerable<int> M() { await Task.CompletedTask; yield return 3; }
                Diagnostic(ErrorCode.ERR_MissingPredefinedMember, "{ await Task.CompletedTask; yield return 3; }").WithArguments("System.Threading.Tasks.ManualResetValueTaskSourceLogic`1", "get_Version").WithLocation(5, 64)
                );

            VerifyMissingMember(WellKnownMember.System_Threading_Tasks_ManualResetValueTaskSourceLogic_T__OnCompleted,
                // (5,64): error CS0656: Missing compiler required member 'System.Threading.Tasks.ManualResetValueTaskSourceLogic`1.OnCompleted'
                //     async System.Collections.Generic.IAsyncEnumerable<int> M() { await Task.CompletedTask; yield return 3; }
                Diagnostic(ErrorCode.ERR_MissingPredefinedMember, "{ await Task.CompletedTask; yield return 3; }").WithArguments("System.Threading.Tasks.ManualResetValueTaskSourceLogic`1", "OnCompleted").WithLocation(5, 64)
                );

            VerifyMissingMember(WellKnownMember.System_Threading_Tasks_ManualResetValueTaskSourceLogic_T__Reset,
                // (5,64): error CS0656: Missing compiler required member 'System.Threading.Tasks.ManualResetValueTaskSourceLogic`1.Reset'
                //     async System.Collections.Generic.IAsyncEnumerable<int> M() { await Task.CompletedTask; yield return 3; }
                Diagnostic(ErrorCode.ERR_MissingPredefinedMember, "{ await Task.CompletedTask; yield return 3; }").WithArguments("System.Threading.Tasks.ManualResetValueTaskSourceLogic`1", "Reset").WithLocation(5, 64)
                );

            VerifyMissingMember(WellKnownMember.System_Threading_Tasks_ManualResetValueTaskSourceLogic_T__SetException,
                // (5,64): error CS0656: Missing compiler required member 'System.Threading.Tasks.ManualResetValueTaskSourceLogic`1.SetException'
                //     async System.Collections.Generic.IAsyncEnumerable<int> M() { await Task.CompletedTask; yield return 3; }
                Diagnostic(ErrorCode.ERR_MissingPredefinedMember, "{ await Task.CompletedTask; yield return 3; }").WithArguments("System.Threading.Tasks.ManualResetValueTaskSourceLogic`1", "SetException").WithLocation(5, 64)
                );

            VerifyMissingMember(WellKnownMember.System_Threading_Tasks_ManualResetValueTaskSourceLogic_T__SetResult,
                // (5,64): error CS0656: Missing compiler required member 'System.Threading.Tasks.ManualResetValueTaskSourceLogic`1.SetResult'
                //     async System.Collections.Generic.IAsyncEnumerable<int> M() { await Task.CompletedTask; yield return 3; }
                Diagnostic(ErrorCode.ERR_MissingPredefinedMember, "{ await Task.CompletedTask; yield return 3; }").WithArguments("System.Threading.Tasks.ManualResetValueTaskSourceLogic`1", "SetResult").WithLocation(5, 64)
                );

            VerifyMissingType(WellKnownType.System_Threading_Tasks_ManualResetValueTaskSourceLogic_T,
                // (5,64): error CS0656: Missing compiler required member 'System.Threading.Tasks.ManualResetValueTaskSourceLogic`1..ctor'
                //     async System.Collections.Generic.IAsyncEnumerable<int> M() { await Task.CompletedTask; yield return 3; }
                Diagnostic(ErrorCode.ERR_MissingPredefinedMember, "{ await Task.CompletedTask; yield return 3; }").WithArguments("System.Threading.Tasks.ManualResetValueTaskSourceLogic`1", ".ctor").WithLocation(5, 64),
                // (5,64): error CS0656: Missing compiler required member 'System.Threading.Tasks.ManualResetValueTaskSourceLogic`1.GetResult'
                //     async System.Collections.Generic.IAsyncEnumerable<int> M() { await Task.CompletedTask; yield return 3; }
                Diagnostic(ErrorCode.ERR_MissingPredefinedMember, "{ await Task.CompletedTask; yield return 3; }").WithArguments("System.Threading.Tasks.ManualResetValueTaskSourceLogic`1", "GetResult").WithLocation(5, 64),
                // (5,64): error CS0656: Missing compiler required member 'System.Threading.Tasks.ManualResetValueTaskSourceLogic`1.GetStatus'
                //     async System.Collections.Generic.IAsyncEnumerable<int> M() { await Task.CompletedTask; yield return 3; }
                Diagnostic(ErrorCode.ERR_MissingPredefinedMember, "{ await Task.CompletedTask; yield return 3; }").WithArguments("System.Threading.Tasks.ManualResetValueTaskSourceLogic`1", "GetStatus").WithLocation(5, 64),
                // (5,64): error CS0656: Missing compiler required member 'System.Threading.Tasks.ManualResetValueTaskSourceLogic`1.get_Version'
                //     async System.Collections.Generic.IAsyncEnumerable<int> M() { await Task.CompletedTask; yield return 3; }
                Diagnostic(ErrorCode.ERR_MissingPredefinedMember, "{ await Task.CompletedTask; yield return 3; }").WithArguments("System.Threading.Tasks.ManualResetValueTaskSourceLogic`1", "get_Version").WithLocation(5, 64),
                // (5,64): error CS0656: Missing compiler required member 'System.Threading.Tasks.ManualResetValueTaskSourceLogic`1.OnCompleted'
                //     async System.Collections.Generic.IAsyncEnumerable<int> M() { await Task.CompletedTask; yield return 3; }
                Diagnostic(ErrorCode.ERR_MissingPredefinedMember, "{ await Task.CompletedTask; yield return 3; }").WithArguments("System.Threading.Tasks.ManualResetValueTaskSourceLogic`1", "OnCompleted").WithLocation(5, 64),
                // (5,64): error CS0656: Missing compiler required member 'System.Threading.Tasks.ManualResetValueTaskSourceLogic`1.Reset'
                //     async System.Collections.Generic.IAsyncEnumerable<int> M() { await Task.CompletedTask; yield return 3; }
                Diagnostic(ErrorCode.ERR_MissingPredefinedMember, "{ await Task.CompletedTask; yield return 3; }").WithArguments("System.Threading.Tasks.ManualResetValueTaskSourceLogic`1", "Reset").WithLocation(5, 64),
                // (5,64): error CS0656: Missing compiler required member 'System.Threading.Tasks.ManualResetValueTaskSourceLogic`1.SetException'
                //     async System.Collections.Generic.IAsyncEnumerable<int> M() { await Task.CompletedTask; yield return 3; }
                Diagnostic(ErrorCode.ERR_MissingPredefinedMember, "{ await Task.CompletedTask; yield return 3; }").WithArguments("System.Threading.Tasks.ManualResetValueTaskSourceLogic`1", "SetException").WithLocation(5, 64),
                // (5,64): error CS0656: Missing compiler required member 'System.Threading.Tasks.ManualResetValueTaskSourceLogic`1.SetResult'
                //     async System.Collections.Generic.IAsyncEnumerable<int> M() { await Task.CompletedTask; yield return 3; }
                Diagnostic(ErrorCode.ERR_MissingPredefinedMember, "{ await Task.CompletedTask; yield return 3; }").WithArguments("System.Threading.Tasks.ManualResetValueTaskSourceLogic`1", "SetResult").WithLocation(5, 64)
                );
        }

        [Fact]
        public void MissingTypeAndMembers_IValueTaskSource()
        {
            VerifyMissingMember(WellKnownMember.System_Threading_Tasks_Sources_IValueTaskSource_T__GetResult,
                // (5,64): error CS0656: Missing compiler required member 'System.Threading.Tasks.Sources.IValueTaskSource`1.GetResult'
                //     async System.Collections.Generic.IAsyncEnumerable<int> M() { await Task.CompletedTask; yield return 3; }
                Diagnostic(ErrorCode.ERR_MissingPredefinedMember, "{ await Task.CompletedTask; yield return 3; }").WithArguments("System.Threading.Tasks.Sources.IValueTaskSource`1", "GetResult").WithLocation(5, 64)
                );

            VerifyMissingMember(WellKnownMember.System_Threading_Tasks_Sources_IValueTaskSource_T__GetStatus,
                // (5,64): error CS0656: Missing compiler required member 'System.Threading.Tasks.Sources.IValueTaskSource`1.GetStatus'
                //     async System.Collections.Generic.IAsyncEnumerable<int> M() { await Task.CompletedTask; yield return 3; }
                Diagnostic(ErrorCode.ERR_MissingPredefinedMember, "{ await Task.CompletedTask; yield return 3; }").WithArguments("System.Threading.Tasks.Sources.IValueTaskSource`1", "GetStatus").WithLocation(5, 64)
                );

            VerifyMissingMember(WellKnownMember.System_Threading_Tasks_Sources_IValueTaskSource_T__OnCompleted,
                // (5,64): error CS0656: Missing compiler required member 'System.Threading.Tasks.Sources.IValueTaskSource`1.OnCompleted'
                //     async System.Collections.Generic.IAsyncEnumerable<int> M() { await Task.CompletedTask; yield return 3; }
                Diagnostic(ErrorCode.ERR_MissingPredefinedMember, "{ await Task.CompletedTask; yield return 3; }").WithArguments("System.Threading.Tasks.Sources.IValueTaskSource`1", "OnCompleted").WithLocation(5, 64)
                );

            VerifyMissingType(WellKnownType.System_Threading_Tasks_Sources_IValueTaskSource_T,
                // (5,64): error CS0656: Missing compiler required member 'System.Threading.Tasks.ValueTask`1..ctor'
                //     async System.Collections.Generic.IAsyncEnumerable<int> M() { await Task.CompletedTask; yield return 3; }
                Diagnostic(ErrorCode.ERR_MissingPredefinedMember, "{ await Task.CompletedTask; yield return 3; }").WithArguments("System.Threading.Tasks.ValueTask`1", ".ctor").WithLocation(5, 64),
                // (5,64): error CS0656: Missing compiler required member 'System.Threading.Tasks.Sources.IValueTaskSource`1.GetResult'
                //     async System.Collections.Generic.IAsyncEnumerable<int> M() { await Task.CompletedTask; yield return 3; }
                Diagnostic(ErrorCode.ERR_MissingPredefinedMember, "{ await Task.CompletedTask; yield return 3; }").WithArguments("System.Threading.Tasks.Sources.IValueTaskSource`1", "GetResult").WithLocation(5, 64),
                // (5,64): error CS0656: Missing compiler required member 'System.Threading.Tasks.Sources.IValueTaskSource`1.GetStatus'
                //     async System.Collections.Generic.IAsyncEnumerable<int> M() { await Task.CompletedTask; yield return 3; }
                Diagnostic(ErrorCode.ERR_MissingPredefinedMember, "{ await Task.CompletedTask; yield return 3; }").WithArguments("System.Threading.Tasks.Sources.IValueTaskSource`1", "GetStatus").WithLocation(5, 64),
                // (5,64): error CS0656: Missing compiler required member 'System.Threading.Tasks.Sources.IValueTaskSource`1.OnCompleted'
                //     async System.Collections.Generic.IAsyncEnumerable<int> M() { await Task.CompletedTask; yield return 3; }
                Diagnostic(ErrorCode.ERR_MissingPredefinedMember, "{ await Task.CompletedTask; yield return 3; }").WithArguments("System.Threading.Tasks.Sources.IValueTaskSource`1", "OnCompleted").WithLocation(5, 64)
                );
        }

        [Fact]
        public void MissingTypeAndMembers_IAsyncStateMachine()
        {
            VerifyMissingMember(WellKnownMember.System_Runtime_CompilerServices_IAsyncStateMachine_MoveNext,
                // (5,64): error CS0656: Missing compiler required member 'System.Runtime.CompilerServices.IAsyncStateMachine.MoveNext'
                //     async System.Collections.Generic.IAsyncEnumerable<int> M() { await Task.CompletedTask; yield return 3; }
                Diagnostic(ErrorCode.ERR_MissingPredefinedMember, "{ await Task.CompletedTask; yield return 3; }").WithArguments("System.Runtime.CompilerServices.IAsyncStateMachine", "MoveNext").WithLocation(5, 64)
                );

            VerifyMissingMember(WellKnownMember.System_Runtime_CompilerServices_IAsyncStateMachine_SetStateMachine,
                // (5,64): error CS0656: Missing compiler required member 'System.Runtime.CompilerServices.IAsyncStateMachine.SetStateMachine'
                //     async System.Collections.Generic.IAsyncEnumerable<int> M() { await Task.CompletedTask; yield return 3; }
                Diagnostic(ErrorCode.ERR_MissingPredefinedMember, "{ await Task.CompletedTask; yield return 3; }").WithArguments("System.Runtime.CompilerServices.IAsyncStateMachine", "SetStateMachine").WithLocation(5, 64)
                );

            VerifyMissingType(WellKnownType.System_Runtime_CompilerServices_IAsyncStateMachine,
                // (5,64): error CS0656: Missing compiler required member 'System.Runtime.CompilerServices.AsyncVoidMethodBuilder.SetStateMachine'
                //     async System.Collections.Generic.IAsyncEnumerable<int> M() { await Task.CompletedTask; yield return 3; }
                Diagnostic(ErrorCode.ERR_MissingPredefinedMember, "{ await Task.CompletedTask; yield return 3; }").WithArguments("System.Runtime.CompilerServices.AsyncVoidMethodBuilder", "SetStateMachine").WithLocation(5, 64),
                // (5,64): error CS0656: Missing compiler required member 'System.Runtime.CompilerServices.IAsyncStateMachine.MoveNext'
                //     async System.Collections.Generic.IAsyncEnumerable<int> M() { await Task.CompletedTask; yield return 3; }
                Diagnostic(ErrorCode.ERR_MissingPredefinedMember, "{ await Task.CompletedTask; yield return 3; }").WithArguments("System.Runtime.CompilerServices.IAsyncStateMachine", "MoveNext").WithLocation(5, 64),
                // (5,64): error CS0656: Missing compiler required member 'System.Runtime.CompilerServices.IAsyncStateMachine.SetStateMachine'
                //     async System.Collections.Generic.IAsyncEnumerable<int> M() { await Task.CompletedTask; yield return 3; }
                Diagnostic(ErrorCode.ERR_MissingPredefinedMember, "{ await Task.CompletedTask; yield return 3; }").WithArguments("System.Runtime.CompilerServices.IAsyncStateMachine", "SetStateMachine").WithLocation(5, 64)
                );
        }

        [Fact]
        public void MissingTypeAndMembers_IAsyncEnumerable()
        {
            VerifyMissingMember(WellKnownMember.System_Collections_Generic_IAsyncEnumerable_T__GetAsyncEnumerator,
                // (5,64): error CS0656: Missing compiler required member 'System.Collections.Generic.IAsyncEnumerable`1.GetAsyncEnumerator'
                //     async System.Collections.Generic.IAsyncEnumerable<int> M() { await Task.CompletedTask; yield return 3; }
                Diagnostic(ErrorCode.ERR_MissingPredefinedMember, "{ await Task.CompletedTask; yield return 3; }").WithArguments("System.Collections.Generic.IAsyncEnumerable`1", "GetAsyncEnumerator").WithLocation(5, 64)
                );

            VerifyMissingType(WellKnownType.System_Collections_Generic_IAsyncEnumerable_T,
                // (5,60): error CS1983: The return type of an async method must be void, Task, Task<T>, a task-like type, or IAsyncEnumerable<T>
                //     async System.Collections.Generic.IAsyncEnumerable<int> M() { await Task.CompletedTask; yield return 3; }
                Diagnostic(ErrorCode.ERR_BadAsyncReturn, "M").WithLocation(5, 60),
                // (5,60): error CS1624: The body of 'C.M()' cannot be an iterator block because 'IAsyncEnumerable<int>' is not an iterator interface type
                //     async System.Collections.Generic.IAsyncEnumerable<int> M() { await Task.CompletedTask; yield return 3; }
                Diagnostic(ErrorCode.ERR_BadIteratorReturn, "M").WithArguments("C.M()", "System.Collections.Generic.IAsyncEnumerable<int>").WithLocation(5, 60)
                );
        }

        [Fact]
        public void MissingType_ValueTaskSourceStatus()
        {
            VerifyMissingType(WellKnownType.System_Threading_Tasks_Sources_ValueTaskSourceStatus,
                // (5,64): error CS0656: Missing compiler required member 'System.Threading.Tasks.ManualResetValueTaskSourceLogic`1.GetStatus'
                //     async System.Collections.Generic.IAsyncEnumerable<int> M() { await Task.CompletedTask; yield return 3; }
                Diagnostic(ErrorCode.ERR_MissingPredefinedMember, "{ await Task.CompletedTask; yield return 3; }").WithArguments("System.Threading.Tasks.ManualResetValueTaskSourceLogic`1", "GetStatus").WithLocation(5, 64),
                // (5,64): error CS0656: Missing compiler required member 'System.Threading.Tasks.Sources.IValueTaskSource`1.GetStatus'
                //     async System.Collections.Generic.IAsyncEnumerable<int> M() { await Task.CompletedTask; yield return 3; }
                Diagnostic(ErrorCode.ERR_MissingPredefinedMember, "{ await Task.CompletedTask; yield return 3; }").WithArguments("System.Threading.Tasks.Sources.IValueTaskSource`1", "GetStatus").WithLocation(5, 64)
                );
        }

        [Fact]
        public void MissingType_ValueTaskSourceOnCompletedFlags()
        {
            VerifyMissingType(WellKnownType.System_Threading_Tasks_Sources_ValueTaskSourceOnCompletedFlags,
                // (5,64): error CS0656: Missing compiler required member 'System.Threading.Tasks.ManualResetValueTaskSourceLogic`1.OnCompleted'
                //     async System.Collections.Generic.IAsyncEnumerable<int> M() { await Task.CompletedTask; yield return 3; }
                Diagnostic(ErrorCode.ERR_MissingPredefinedMember, "{ await Task.CompletedTask; yield return 3; }").WithArguments("System.Threading.Tasks.ManualResetValueTaskSourceLogic`1", "OnCompleted").WithLocation(5, 64),
                // (5,64): error CS0656: Missing compiler required member 'System.Threading.Tasks.Sources.IValueTaskSource`1.OnCompleted'
                //     async System.Collections.Generic.IAsyncEnumerable<int> M() { await Task.CompletedTask; yield return 3; }
                Diagnostic(ErrorCode.ERR_MissingPredefinedMember, "{ await Task.CompletedTask; yield return 3; }").WithArguments("System.Threading.Tasks.Sources.IValueTaskSource`1", "OnCompleted").WithLocation(5, 64)
                );
        }

        [Fact]
        public void MissingType_IAsyncIEnumerable_LocalFunction()
        {
            string source = @"
class C
{
    void Method()
    {
        _ = local();

        async System.Collections.Generic.IAsyncEnumerable<int> local()
        {
            await System.Threading.Tasks.Task.CompletedTask;
            yield return 3;
        }
    }
}
namespace System.Collections.Generic
{
    public interface IAsyncEnumerator<out T> : System.IAsyncDisposable
    {
        System.Threading.Tasks.ValueTask<bool> WaitForNextAsync();
        T TryGetNext(out bool success);
    }
}
namespace System
{
    public interface IAsyncDisposable
    {
        System.Threading.Tasks.ValueTask DisposeAsync();
    }
}
";
            var comp = CreateCompilationWithTasksExtensions(new[] { source });
            comp.VerifyDiagnostics(
                // (8,42): error CS0234: The type or namespace name 'IAsyncEnumerable<>' does not exist in the namespace 'System.Collections.Generic' (are you missing an assembly reference?)
                //         async System.Collections.Generic.IAsyncEnumerable<int> local()
                Diagnostic(ErrorCode.ERR_DottedTypeNameNotFoundInNS, "IAsyncEnumerable<int>").WithArguments("IAsyncEnumerable<>", "System.Collections.Generic").WithLocation(8, 42),
                // (8,64): error CS1983: The return type of an async method must be void, Task, Task<T>, a task-like type, or IAsyncEnumerable<T>
                //         async System.Collections.Generic.IAsyncEnumerable<int> local()
                Diagnostic(ErrorCode.ERR_BadAsyncReturn, "local").WithLocation(8, 64)
                );
        }

        [Fact]
        public void MissingTypeAndMembers_IAsyncEnumerator()
        {
            VerifyMissingMember(WellKnownMember.System_Collections_Generic_IAsyncEnumerator_T__TryGetNext,
                // (5,64): error CS0656: Missing compiler required member 'System.Collections.Generic.IAsyncEnumerator`1.TryGetNext'
                //     async System.Collections.Generic.IAsyncEnumerable<int> M() { await Task.CompletedTask; yield return 3; }
                Diagnostic(ErrorCode.ERR_MissingPredefinedMember, "{ await Task.CompletedTask; yield return 3; }").WithArguments("System.Collections.Generic.IAsyncEnumerator`1", "TryGetNext").WithLocation(5, 64)
                );

            VerifyMissingMember(WellKnownMember.System_Collections_Generic_IAsyncEnumerator_T__WaitForNextAsync,
                // (5,64): error CS0656: Missing compiler required member 'System.Collections.Generic.IAsyncEnumerator`1.WaitForNextAsync'
                //     async System.Collections.Generic.IAsyncEnumerable<int> M() { await Task.CompletedTask; yield return 3; }
                Diagnostic(ErrorCode.ERR_MissingPredefinedMember, "{ await Task.CompletedTask; yield return 3; }").WithArguments("System.Collections.Generic.IAsyncEnumerator`1", "WaitForNextAsync").WithLocation(5, 64)
                );

            VerifyMissingType(WellKnownType.System_Collections_Generic_IAsyncEnumerator_T,
                // (5,64): error CS0656: Missing compiler required member 'System.Collections.Generic.IAsyncEnumerable`1.GetAsyncEnumerator'
                //     async System.Collections.Generic.IAsyncEnumerable<int> M() { await Task.CompletedTask; yield return 3; }
                Diagnostic(ErrorCode.ERR_MissingPredefinedMember, "{ await Task.CompletedTask; yield return 3; }").WithArguments("System.Collections.Generic.IAsyncEnumerable`1", "GetAsyncEnumerator").WithLocation(5, 64),
                // (5,64): error CS0656: Missing compiler required member 'System.Collections.Generic.IAsyncEnumerator`1.WaitForNextAsync'
                //     async System.Collections.Generic.IAsyncEnumerable<int> M() { await Task.CompletedTask; yield return 3; }
                Diagnostic(ErrorCode.ERR_MissingPredefinedMember, "{ await Task.CompletedTask; yield return 3; }").WithArguments("System.Collections.Generic.IAsyncEnumerator`1", "WaitForNextAsync").WithLocation(5, 64),
                // (5,64): error CS0656: Missing compiler required member 'System.Collections.Generic.IAsyncEnumerator`1.TryGetNext'
                //     async System.Collections.Generic.IAsyncEnumerable<int> M() { await Task.CompletedTask; yield return 3; }
                Diagnostic(ErrorCode.ERR_MissingPredefinedMember, "{ await Task.CompletedTask; yield return 3; }").WithArguments("System.Collections.Generic.IAsyncEnumerator`1", "TryGetNext").WithLocation(5, 64)
                );
        }

        [Fact]
        public void MissingTypeAndMembers_IAsyncDisposable()
        {
            VerifyMissingMember(WellKnownMember.System_IAsyncDisposable__DisposeAsync,
                // (5,64): error CS0656: Missing compiler required member 'System.IAsyncDisposable.DisposeAsync'
                //     async System.Collections.Generic.IAsyncEnumerable<int> M() { await Task.CompletedTask; yield return 3; }
                Diagnostic(ErrorCode.ERR_MissingPredefinedMember, "{ await Task.CompletedTask; yield return 3; }").WithArguments("System.IAsyncDisposable", "DisposeAsync").WithLocation(5, 64)
                );

            VerifyMissingType(WellKnownType.System_IAsyncDisposable,
                // (5,64): error CS0656: Missing compiler required member 'System.IAsyncDisposable.DisposeAsync'
                //     async System.Collections.Generic.IAsyncEnumerable<int> M() { await Task.CompletedTask; yield return 3; }
                Diagnostic(ErrorCode.ERR_MissingPredefinedMember, "{ await Task.CompletedTask; yield return 3; }").WithArguments("System.IAsyncDisposable", "DisposeAsync").WithLocation(5, 64)
                );
        }

        [Fact]
        public void MissingTypeAndMembers_ValueTask()
        {
            VerifyMissingMember(WellKnownMember.System_Threading_Tasks_ValueTask_T__ctor,
                // (5,64): error CS0656: Missing compiler required member 'System.Threading.Tasks.ValueTask`1..ctor'
                //     async System.Collections.Generic.IAsyncEnumerable<int> M() { await Task.CompletedTask; yield return 3; }
                Diagnostic(ErrorCode.ERR_MissingPredefinedMember, "{ await Task.CompletedTask; yield return 3; }").WithArguments("System.Threading.Tasks.ValueTask`1", ".ctor").WithLocation(5, 64)
                );

            VerifyMissingType(WellKnownType.System_Threading_Tasks_ValueTask_T,
                // (5,64): error CS0656: Missing compiler required member 'System.Collections.Generic.IAsyncEnumerator`1.WaitForNextAsync'
                //     async System.Collections.Generic.IAsyncEnumerable<int> M() { await Task.CompletedTask; yield return 3; }
                Diagnostic(ErrorCode.ERR_MissingPredefinedMember, "{ await Task.CompletedTask; yield return 3; }").WithArguments("System.Collections.Generic.IAsyncEnumerator`1", "WaitForNextAsync").WithLocation(5, 64),
                // (5,64): error CS0656: Missing compiler required member 'System.Threading.Tasks.ValueTask`1..ctor'
                //     async System.Collections.Generic.IAsyncEnumerable<int> M() { await Task.CompletedTask; yield return 3; }
                Diagnostic(ErrorCode.ERR_MissingPredefinedMember, "{ await Task.CompletedTask; yield return 3; }").WithArguments("System.Threading.Tasks.ValueTask`1", ".ctor").WithLocation(5, 64)
                );
        }

        [Fact]
        public void MissingTypeAndMembers_IStrongBox()
        {
            VerifyMissingMember(WellKnownMember.System_Runtime_CompilerServices_IStrongBox_T__get_Value,
                // (5,64): error CS0656: Missing compiler required member 'System.Runtime.CompilerServices.IStrongBox`1.get_Value'
                //     async System.Collections.Generic.IAsyncEnumerable<int> M() { await Task.CompletedTask; yield return 3; }
                Diagnostic(ErrorCode.ERR_MissingPredefinedMember, "{ await Task.CompletedTask; yield return 3; }").WithArguments("System.Runtime.CompilerServices.IStrongBox`1", "get_Value").WithLocation(5, 64)
                );

            VerifyMissingMember(WellKnownMember.System_Runtime_CompilerServices_IStrongBox_T__Value,
                // (5,64): error CS0656: Missing compiler required member 'System.Runtime.CompilerServices.IStrongBox`1.Value'
                //     async System.Collections.Generic.IAsyncEnumerable<int> M() { await Task.CompletedTask; yield return 3; }
                Diagnostic(ErrorCode.ERR_MissingPredefinedMember, "{ await Task.CompletedTask; yield return 3; }").WithArguments("System.Runtime.CompilerServices.IStrongBox`1", "Value").WithLocation(5, 64)
                );

            VerifyMissingType(WellKnownType.System_Runtime_CompilerServices_IStrongBox_T,
                // (5,64): error CS0656: Missing compiler required member 'System.Threading.Tasks.ManualResetValueTaskSourceLogic`1..ctor'
                //     async System.Collections.Generic.IAsyncEnumerable<int> M() { await Task.CompletedTask; yield return 3; }
                Diagnostic(ErrorCode.ERR_MissingPredefinedMember, "{ await Task.CompletedTask; yield return 3; }").WithArguments("System.Threading.Tasks.ManualResetValueTaskSourceLogic`1", ".ctor").WithLocation(5, 64),
                // (5,64): error CS0656: Missing compiler required member 'System.Runtime.CompilerServices.IStrongBox`1.get_Value'
                //     async System.Collections.Generic.IAsyncEnumerable<int> M() { await Task.CompletedTask; yield return 3; }
                Diagnostic(ErrorCode.ERR_MissingPredefinedMember, "{ await Task.CompletedTask; yield return 3; }").WithArguments("System.Runtime.CompilerServices.IStrongBox`1", "get_Value").WithLocation(5, 64),
                // (5,64): error CS0656: Missing compiler required member 'System.Runtime.CompilerServices.IStrongBox`1.Value'
                //     async System.Collections.Generic.IAsyncEnumerable<int> M() { await Task.CompletedTask; yield return 3; }
                Diagnostic(ErrorCode.ERR_MissingPredefinedMember, "{ await Task.CompletedTask; yield return 3; }").WithArguments("System.Runtime.CompilerServices.IStrongBox`1", "Value").WithLocation(5, 64)
                );
        }

        [Fact]
        public void AsyncIteratorWithBreak()
        {
            string source = @"
class C
{
    async System.Collections.Generic.IAsyncEnumerable<int> M()
    {
        await System.Threading.Tasks.Task.CompletedTask;
        yield return 3;
        break;
    }
}";
            var comp = CreateCompilationWithTasksExtensions(new[] { source, s_common });
            comp.VerifyDiagnostics(
                // (8,9): error CS0139: No enclosing loop out of which to break or continue
                //         break;
                Diagnostic(ErrorCode.ERR_NoBreakOrCont, "break;").WithLocation(8, 9)
                );
        }

        [Fact]
        public void AsyncIteratorWithReturnInt()
        {
            string source = @"
class C
{
    async System.Collections.Generic.IAsyncEnumerable<int> M()
    {
        await System.Threading.Tasks.Task.CompletedTask;
        yield return 3;
        return 1;
    }
    async System.Collections.Generic.IAsyncEnumerable<int> M2()
    {
        return 4;
    }
}";
            var comp = CreateCompilationWithTasksExtensions(new[] { source, s_common });
            comp.VerifyDiagnostics(
                // (8,9): error CS1622: Cannot return a value from an iterator. Use the yield return statement to return a value, or yield break to end the iteration.
                //         return 1;
                Diagnostic(ErrorCode.ERR_ReturnInIterator, "return").WithLocation(8, 9),
                // (10,60): warning CS1998: This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
                //     async System.Collections.Generic.IAsyncEnumerable<int> M2()
                Diagnostic(ErrorCode.WRN_AsyncLacksAwaits, "M2").WithLocation(10, 60),
                // (12,9): error CS1622: Cannot return a value from an iterator. Use the yield return statement to return a value, or yield break to end the iteration.
                //         return 4;
                Diagnostic(ErrorCode.ERR_ReturnInIterator, "return").WithLocation(12, 9)
                );
        }

        [Fact]
        public void AsyncIteratorWithReturnNull()
        {
            string source = @"
class C
{
    async System.Collections.Generic.IAsyncEnumerable<int> M()
    {
        await System.Threading.Tasks.Task.CompletedTask;
        yield return 3;
        return null;
    }
    async System.Collections.Generic.IAsyncEnumerable<int> M2()
    {
        return null;
    }
}";
            var comp = CreateCompilationWithTasksExtensions(new[] { source, s_common });
            comp.VerifyDiagnostics(
                // (8,9): error CS1622: Cannot return a value from an iterator. Use the yield return statement to return a value, or yield break to end the iteration.
                //         return null;
                Diagnostic(ErrorCode.ERR_ReturnInIterator, "return").WithLocation(8, 9),
                // (10,60): warning CS1998: This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
                //     async System.Collections.Generic.IAsyncEnumerable<int> M2()
                Diagnostic(ErrorCode.WRN_AsyncLacksAwaits, "M2").WithLocation(10, 60),
                // (12,9): error CS1622: Cannot return a value from an iterator. Use the yield return statement to return a value, or yield break to end the iteration.
                //         return null;
                Diagnostic(ErrorCode.ERR_ReturnInIterator, "return").WithLocation(12, 9)
                );
        }

        [Fact]
        public void AsyncIteratorWithReturnRef()
        {
            string source = @"
class C
{
    async System.Collections.Generic.IAsyncEnumerable<int> M(ref string s)
    {
        await System.Threading.Tasks.Task.CompletedTask;
        yield return 3;
        return ref s;
    }
    async System.Collections.Generic.IAsyncEnumerable<int> M2(ref string s2)
    {
        return ref s2;
    }
}";
            var comp = CreateCompilationWithTasksExtensions(new[] { source, s_common });
            comp.VerifyDiagnostics(
                // (4,73): error CS1988: Async methods cannot have ref, in or out parameters
                //     async System.Collections.Generic.IAsyncEnumerable<int> M(ref string s)
                Diagnostic(ErrorCode.ERR_BadAsyncArgType, "s").WithLocation(4, 73),
                // (4,73): error CS1623: Iterators cannot have ref, in or out parameters
                //     async System.Collections.Generic.IAsyncEnumerable<int> M(ref string s)
                Diagnostic(ErrorCode.ERR_BadIteratorArgType, "s").WithLocation(4, 73),
                // (8,9): error CS1622: Cannot return a value from an iterator. Use the yield return statement to return a value, or yield break to end the iteration.
                //         return ref s;
                Diagnostic(ErrorCode.ERR_ReturnInIterator, "return").WithLocation(8, 9),
                // (10,60): warning CS1998: This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
                //     async System.Collections.Generic.IAsyncEnumerable<int> M2(ref string s2)
                Diagnostic(ErrorCode.WRN_AsyncLacksAwaits, "M2").WithLocation(10, 60),
                // (10,74): error CS1988: Async methods cannot have ref, in or out parameters
                //     async System.Collections.Generic.IAsyncEnumerable<int> M2(ref string s2)
                Diagnostic(ErrorCode.ERR_BadAsyncArgType, "s2").WithLocation(10, 74),
                // (12,9): error CS8149: By-reference returns may only be used in methods that return by reference
                //         return ref s2;
                Diagnostic(ErrorCode.ERR_MustNotHaveRefReturn, "return").WithLocation(12, 9)
                );
        }

        [Fact]
        public void AsyncIteratorWithReturnDefault()
        {
            string source = @"
class C
{
    async System.Collections.Generic.IAsyncEnumerable<int> M()
    {
        await System.Threading.Tasks.Task.CompletedTask;
        yield return 3;
        return default;
    }
    async System.Collections.Generic.IAsyncEnumerable<int> M2()
    {
        return default;
    }
}";
            var comp = CreateCompilationWithTasksExtensions(new[] { source, s_common });
            comp.VerifyDiagnostics(
                // (8,9): error CS1622: Cannot return a value from an iterator. Use the yield return statement to return a value, or yield break to end the iteration.
                //         return default;
                Diagnostic(ErrorCode.ERR_ReturnInIterator, "return").WithLocation(8, 9),
                // (10,60): warning CS1998: This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
                //     async System.Collections.Generic.IAsyncEnumerable<int> M2()
                Diagnostic(ErrorCode.WRN_AsyncLacksAwaits, "M2").WithLocation(10, 60),
                // (12,9): error CS1622: Cannot return a value from an iterator. Use the yield return statement to return a value, or yield break to end the iteration.
                //         return default;
                Diagnostic(ErrorCode.ERR_ReturnInIterator, "return").WithLocation(12, 9)
                );
        }

        [Fact]
        public void CallingWaitForNextAsyncTwice()
        {
            string source = @"
using static System.Console;
class C
{
    static async System.Collections.Generic.IAsyncEnumerable<int> M()
    {
        Write(""1 "");
        await System.Threading.Tasks.Task.CompletedTask;
        Write(""2 "");
        yield return 4;
        Write(""5 "");
    }
    static async System.Threading.Tasks.Task Main()
    {
        Write(""0 "");
        using await (var enumerator = M().GetAsyncEnumerator())
        {
            var found = await enumerator.WaitForNextAsync();
            if (!found) throw null;
            Write(""3 "");
            found = await enumerator.WaitForNextAsync();
            if (!found) throw null;
            var value = enumerator.TryGetNext(out var success);
            Write($""{value} "");
            await enumerator.WaitForNextAsync();
            Write(""6"");
        }
    }
}";
            var comp = CreateCompilationWithTasksExtensions(new[] { source, s_common }, options: TestOptions.DebugExe);
            comp.VerifyDiagnostics();
            CompileAndVerify(comp, expectedOutput: "0 1 2 3 4 5 6");
        }

        [ConditionalFact(typeof(WindowsOnly), Reason = ConditionalSkipReason.NativePdbRequiresDesktop)]
        public void AsyncIteratorWithAwaitCompletedAndYield()
        {
            string source = @"
using static System.Console;
class C
{
    static async System.Collections.Generic.IAsyncEnumerable<int> M()
    {
        Write(""1 "");
        await System.Threading.Tasks.Task.CompletedTask;
        Write(""2 "");
        yield return 3;
        Write("" 4 "");
    }
    static async System.Threading.Tasks.Task Main()
    {
        Write(""0 "");
        foreach await (var i in M())
        {
            Write(i);
        }
        Write(""5"");
    }
}";
            foreach (var options in new[] { TestOptions.DebugExe, TestOptions.ReleaseExe })
            {
                var comp = CreateCompilationWithTasksExtensions(new[] { source, s_common }, options: options);
                comp.VerifyDiagnostics();
                var verifier = CompileAndVerify(comp, expectedOutput: "0 1 2 3 4 5");

                verifier.VerifyIL("C.M", @"
{
  // Code size       45 (0x2d)
  .maxstack  2
  .locals init (C.<M>d__0 V_0)
  IL_0000:  newobj     ""C.<M>d__0..ctor()""
  IL_0005:  stloc.0
  IL_0006:  ldloc.0
  IL_0007:  call       ""System.Runtime.CompilerServices.AsyncVoidMethodBuilder System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Create()""
  IL_000c:  stfld      ""System.Runtime.CompilerServices.AsyncVoidMethodBuilder C.<M>d__0.<>t__builder""
  IL_0011:  ldloc.0
  IL_0012:  ldc.i4.m1
  IL_0013:  stfld      ""int C.<M>d__0.<>1__state""
  IL_0018:  ldloc.0
  IL_0019:  ldloc.0
  IL_001a:  newobj     ""System.Threading.Tasks.ManualResetValueTaskSourceLogic<bool>..ctor(System.Runtime.CompilerServices.IStrongBox<System.Threading.Tasks.ManualResetValueTaskSourceLogic<bool>>)""
  IL_001f:  stfld      ""System.Threading.Tasks.ManualResetValueTaskSourceLogic<bool> C.<M>d__0.<>v__promiseOfValueOrEnd""
  IL_0024:  ldloc.0
  IL_0025:  ldc.i4.1
  IL_0026:  stfld      ""bool C.<M>d__0.<>w__promiseIsActive""
  IL_002b:  ldloc.0
  IL_002c:  ret
}", sequencePoints: "C.M", source: source);

                verifier.VerifyIL("C.<M>d__0.System.Collections.Generic.IAsyncEnumerator<int>.TryGetNext(out bool)", @"
{
  // Code size       65 (0x41)
  .maxstack  2
  .locals init (C.<M>d__0 V_0)
  IL_0000:  ldarg.0
  IL_0001:  ldfld      ""bool C.<M>d__0.<>w__promiseIsActive""
  IL_0006:  brfalse.s  IL_0011
  IL_0008:  ldarg.0
  IL_0009:  ldc.i4.0
  IL_000a:  stfld      ""bool C.<M>d__0.<>w__promiseIsActive""
  IL_000f:  br.s       IL_0020
  IL_0011:  ldarg.0
  IL_0012:  stloc.0
  IL_0013:  ldarg.0
  IL_0014:  ldflda     ""System.Runtime.CompilerServices.AsyncVoidMethodBuilder C.<M>d__0.<>t__builder""
  IL_0019:  ldloca.s   V_0
  IL_001b:  call       ""void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<C.<M>d__0>(ref C.<M>d__0)""
  IL_0020:  ldarg.0
  IL_0021:  ldfld      ""bool C.<M>d__0.<>w__promiseIsActive""
  IL_0026:  brtrue.s   IL_0032
  IL_0028:  ldarg.0
  IL_0029:  ldfld      ""int C.<M>d__0.<>1__state""
  IL_002e:  ldc.i4.s   -2
  IL_0030:  bne.un.s   IL_0037
  IL_0032:  ldarg.1
  IL_0033:  ldc.i4.0
  IL_0034:  stind.i1
  IL_0035:  ldc.i4.0
  IL_0036:  ret
  IL_0037:  ldarg.1
  IL_0038:  ldc.i4.1
  IL_0039:  stind.i1
  IL_003a:  ldarg.0
  IL_003b:  ldfld      ""int C.<M>d__0.<>2__current""
  IL_0040:  ret
}");
                verifier.VerifyIL("C.<M>d__0.System.Collections.Generic.IAsyncEnumerator<int>.WaitForNextAsync()", @"
{
  // Code size       70 (0x46)
  .maxstack  2
  .locals init (System.Threading.Tasks.ValueTask<bool> V_0,
                C.<M>d__0 V_1)
  IL_0000:  ldarg.0
  IL_0001:  ldfld      ""int C.<M>d__0.<>1__state""
  IL_0006:  ldc.i4.s   -2
  IL_0008:  bne.un.s   IL_0014
  IL_000a:  ldloca.s   V_0
  IL_000c:  initobj    ""System.Threading.Tasks.ValueTask<bool>""
  IL_0012:  ldloc.0
  IL_0013:  ret
  IL_0014:  ldarg.0
  IL_0015:  ldfld      ""bool C.<M>d__0.<>w__promiseIsActive""
  IL_001a:  brfalse.s  IL_0025
  IL_001c:  ldarg.0
  IL_001d:  ldfld      ""int C.<M>d__0.<>1__state""
  IL_0022:  ldc.i4.m1
  IL_0023:  bne.un.s   IL_0034
  IL_0025:  ldarg.0
  IL_0026:  stloc.1
  IL_0027:  ldarg.0
  IL_0028:  ldflda     ""System.Runtime.CompilerServices.AsyncVoidMethodBuilder C.<M>d__0.<>t__builder""
  IL_002d:  ldloca.s   V_1
  IL_002f:  call       ""void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<C.<M>d__0>(ref C.<M>d__0)""
  IL_0034:  ldarg.0
  IL_0035:  ldarg.0
  IL_0036:  ldflda     ""System.Threading.Tasks.ManualResetValueTaskSourceLogic<bool> C.<M>d__0.<>v__promiseOfValueOrEnd""
  IL_003b:  call       ""short System.Threading.Tasks.ManualResetValueTaskSourceLogic<bool>.Version.get""
  IL_0040:  newobj     ""System.Threading.Tasks.ValueTask<bool>..ctor(System.Threading.Tasks.Sources.IValueTaskSource<bool>, short)""
  IL_0045:  ret
}");
                verifier.VerifyIL("C.<M>d__0.System.IAsyncDisposable.DisposeAsync()", @"
{
  // Code size       39 (0x27)
  .maxstack  2
  .locals init (System.Threading.Tasks.ValueTask V_0)
  IL_0000:  ldarg.0
  IL_0001:  ldflda     ""System.Runtime.CompilerServices.AsyncVoidMethodBuilder C.<M>d__0.<>t__builder""
  IL_0006:  call       ""void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.SetResult()""
  IL_000b:  ldarg.0
  IL_000c:  ldflda     ""System.Threading.Tasks.ManualResetValueTaskSourceLogic<bool> C.<M>d__0.<>v__promiseOfValueOrEnd""
  IL_0011:  call       ""void System.Threading.Tasks.ManualResetValueTaskSourceLogic<bool>.Reset()""
  IL_0016:  ldarg.0
  IL_0017:  ldc.i4.m1
  IL_0018:  stfld      ""int C.<M>d__0.<>1__state""
  IL_001d:  ldloca.s   V_0
  IL_001f:  initobj    ""System.Threading.Tasks.ValueTask""
  IL_0025:  ldloc.0
  IL_0026:  ret
}
");
                verifier.VerifyIL("C.<M>d__0.System.Runtime.CompilerServices.IStrongBox<System.Threading.Tasks.ManualResetValueTaskSourceLogic<bool>>.get_Value()", @"
{
  // Code size        7 (0x7)
  .maxstack  1
  IL_0000:  ldarg.0
  IL_0001:  ldflda     ""System.Threading.Tasks.ManualResetValueTaskSourceLogic<bool> C.<M>d__0.<>v__promiseOfValueOrEnd""
  IL_0006:  ret
}");
                verifier.VerifyIL("C.<M>d__0.System.Collections.Generic.IAsyncEnumerable<int>.GetAsyncEnumerator()", @"
{
  // Code size        2 (0x2)
  .maxstack  1
  IL_0000:  ldarg.0
  IL_0001:  ret
}");
                verifier.VerifyIL("C.<M>d__0.System.Threading.Tasks.Sources.IValueTaskSource<bool>.GetResult(short)", @"
{
  // Code size       13 (0xd)
  .maxstack  2
  IL_0000:  ldarg.0
  IL_0001:  ldflda     ""System.Threading.Tasks.ManualResetValueTaskSourceLogic<bool> C.<M>d__0.<>v__promiseOfValueOrEnd""
  IL_0006:  ldarg.1
  IL_0007:  call       ""bool System.Threading.Tasks.ManualResetValueTaskSourceLogic<bool>.GetResult(short)""
  IL_000c:  ret
}");
                verifier.VerifyIL("C.<M>d__0.System.Threading.Tasks.Sources.IValueTaskSource<bool>.GetStatus(short)", @"
{
  // Code size       13 (0xd)
  .maxstack  2
  IL_0000:  ldarg.0
  IL_0001:  ldflda     ""System.Threading.Tasks.ManualResetValueTaskSourceLogic<bool> C.<M>d__0.<>v__promiseOfValueOrEnd""
  IL_0006:  ldarg.1
  IL_0007:  call       ""System.Threading.Tasks.Sources.ValueTaskSourceStatus System.Threading.Tasks.ManualResetValueTaskSourceLogic<bool>.GetStatus(short)""
  IL_000c:  ret
}");
                verifier.VerifyIL("C.<M>d__0.System.Runtime.CompilerServices.IAsyncStateMachine.SetStateMachine(System.Runtime.CompilerServices.IAsyncStateMachine)", @"
{
  // Code size        1 (0x1)
  .maxstack  0
  IL_0000:  ret
}");
                verifier.VerifyIL("C.<M>d__0.System.Threading.Tasks.Sources.IValueTaskSource<bool>.OnCompleted(System.Action<object>, object, short, System.Threading.Tasks.Sources.ValueTaskSourceOnCompletedFlags)", @"
{
  // Code size       17 (0x11)
  .maxstack  5
  IL_0000:  ldarg.0
  IL_0001:  ldflda     ""System.Threading.Tasks.ManualResetValueTaskSourceLogic<bool> C.<M>d__0.<>v__promiseOfValueOrEnd""
  IL_0006:  ldarg.1
  IL_0007:  ldarg.2
  IL_0008:  ldarg.3
  IL_0009:  ldarg.s    V_4
  IL_000b:  call       ""void System.Threading.Tasks.ManualResetValueTaskSourceLogic<bool>.OnCompleted(System.Action<object>, object, short, System.Threading.Tasks.Sources.ValueTaskSourceOnCompletedFlags)""
  IL_0010:  ret
}");
                if (options == TestOptions.DebugExe)
                {
                    verifier.VerifyIL("C.<M>d__0.System.Runtime.CompilerServices.IAsyncStateMachine.MoveNext()", @"
{
  // Code size      317 (0x13d)
  .maxstack  3
  .locals init (int V_0,
                System.Runtime.CompilerServices.TaskAwaiter V_1,
                C.<M>d__0 V_2,
                System.Exception V_3)
  // sequence point: <hidden>
  IL_0000:  ldarg.0
  IL_0001:  ldfld      ""int C.<M>d__0.<>1__state""
  IL_0006:  stloc.0
  .try
  {
    // sequence point: <hidden>
    IL_0007:  ldloc.0
    IL_0008:  brfalse.s  IL_0012
    IL_000a:  br.s       IL_000c
    IL_000c:  ldloc.0
    IL_000d:  ldc.i4.1
    IL_000e:  beq.s      IL_0014
    IL_0010:  br.s       IL_0019
    IL_0012:  br.s       IL_007b
    IL_0014:  br         IL_00cf
    // sequence point: {
    IL_0019:  nop
    // sequence point: Write(""1 "");
    IL_001a:  ldstr      ""1 ""
    IL_001f:  call       ""void System.Console.Write(string)""
    IL_0024:  nop
    // sequence point: await System.Threading.Tasks.Task.CompletedTask;
    IL_0025:  call       ""System.Threading.Tasks.Task System.Threading.Tasks.Task.CompletedTask.get""
    IL_002a:  callvirt   ""System.Runtime.CompilerServices.TaskAwaiter System.Threading.Tasks.Task.GetAwaiter()""
    IL_002f:  stloc.1
    // sequence point: <hidden>
    IL_0030:  ldloca.s   V_1
    IL_0032:  call       ""bool System.Runtime.CompilerServices.TaskAwaiter.IsCompleted.get""
    IL_0037:  brtrue.s   IL_0097
    IL_0039:  ldarg.0
    IL_003a:  ldc.i4.0
    IL_003b:  dup
    IL_003c:  stloc.0
    IL_003d:  stfld      ""int C.<M>d__0.<>1__state""
    // async: yield
    IL_0042:  ldarg.0
    IL_0043:  ldloc.1
    IL_0044:  stfld      ""System.Runtime.CompilerServices.TaskAwaiter C.<M>d__0.<>u__1""
    IL_0049:  ldarg.0
    IL_004a:  ldfld      ""bool C.<M>d__0.<>w__promiseIsActive""
    IL_004f:  brtrue.s   IL_0064
    IL_0051:  ldarg.0
    IL_0052:  ldc.i4.1
    IL_0053:  stfld      ""bool C.<M>d__0.<>w__promiseIsActive""
    IL_0058:  ldarg.0
    IL_0059:  ldflda     ""System.Threading.Tasks.ManualResetValueTaskSourceLogic<bool> C.<M>d__0.<>v__promiseOfValueOrEnd""
    IL_005e:  call       ""void System.Threading.Tasks.ManualResetValueTaskSourceLogic<bool>.Reset()""
    IL_0063:  nop
    IL_0064:  ldarg.0
    IL_0065:  stloc.2
    IL_0066:  ldarg.0
    IL_0067:  ldflda     ""System.Runtime.CompilerServices.AsyncVoidMethodBuilder C.<M>d__0.<>t__builder""
    IL_006c:  ldloca.s   V_1
    IL_006e:  ldloca.s   V_2
    IL_0070:  call       ""void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter, C.<M>d__0>(ref System.Runtime.CompilerServices.TaskAwaiter, ref C.<M>d__0)""
    IL_0075:  nop
    IL_0076:  leave      IL_013c
    // async: resume
    IL_007b:  ldarg.0
    IL_007c:  ldfld      ""System.Runtime.CompilerServices.TaskAwaiter C.<M>d__0.<>u__1""
    IL_0081:  stloc.1
    IL_0082:  ldarg.0
    IL_0083:  ldflda     ""System.Runtime.CompilerServices.TaskAwaiter C.<M>d__0.<>u__1""
    IL_0088:  initobj    ""System.Runtime.CompilerServices.TaskAwaiter""
    IL_008e:  ldarg.0
    IL_008f:  ldc.i4.m1
    IL_0090:  dup
    IL_0091:  stloc.0
    IL_0092:  stfld      ""int C.<M>d__0.<>1__state""
    IL_0097:  ldloca.s   V_1
    IL_0099:  call       ""void System.Runtime.CompilerServices.TaskAwaiter.GetResult()""
    IL_009e:  nop
    // sequence point: Write(""2 "");
    IL_009f:  ldstr      ""2 ""
    IL_00a4:  call       ""void System.Console.Write(string)""
    IL_00a9:  nop
    // sequence point: yield return 3;
    IL_00aa:  ldarg.0
    IL_00ab:  ldc.i4.3
    IL_00ac:  stfld      ""int C.<M>d__0.<>2__current""
    IL_00b1:  ldarg.0
    IL_00b2:  ldc.i4.1
    IL_00b3:  stfld      ""int C.<M>d__0.<>1__state""
    IL_00b8:  ldarg.0
    IL_00b9:  ldfld      ""bool C.<M>d__0.<>w__promiseIsActive""
    IL_00be:  brfalse.s  IL_00cd
    IL_00c0:  ldarg.0
    IL_00c1:  ldflda     ""System.Threading.Tasks.ManualResetValueTaskSourceLogic<bool> C.<M>d__0.<>v__promiseOfValueOrEnd""
    IL_00c6:  ldc.i4.1
    IL_00c7:  call       ""void System.Threading.Tasks.ManualResetValueTaskSourceLogic<bool>.SetResult(bool)""
    IL_00cc:  nop
    IL_00cd:  leave.s    IL_013c
    // sequence point: Write("" 4 "");
    IL_00cf:  ldstr      "" 4 ""
    IL_00d4:  call       ""void System.Console.Write(string)""
    IL_00d9:  nop
    IL_00da:  leave.s    IL_010c
  }
  filter
  {
    // sequence point: <hidden>
    IL_00dc:  isinst     ""System.Exception""
    IL_00e1:  dup
    IL_00e2:  brtrue.s   IL_00e8
    IL_00e4:  pop
    IL_00e5:  ldc.i4.0
    IL_00e6:  br.s       IL_00fa
    IL_00e8:  stloc.3
    IL_00e9:  ldarg.0
    IL_00ea:  ldc.i4.s   -2
    IL_00ec:  stfld      ""int C.<M>d__0.<>1__state""
    IL_00f1:  ldarg.0
    IL_00f2:  ldfld      ""bool C.<M>d__0.<>w__promiseIsActive""
    IL_00f7:  ldc.i4.0
    IL_00f8:  cgt.un
    IL_00fa:  endfilter
  }  // end filter
  {  // handler
    // sequence point: <hidden>
    IL_00fc:  pop
    IL_00fd:  ldarg.0
    IL_00fe:  ldflda     ""System.Threading.Tasks.ManualResetValueTaskSourceLogic<bool> C.<M>d__0.<>v__promiseOfValueOrEnd""
    IL_0103:  ldloc.3
    IL_0104:  call       ""void System.Threading.Tasks.ManualResetValueTaskSourceLogic<bool>.SetException(System.Exception)""
    IL_0109:  nop
    IL_010a:  leave.s    IL_013c
  }
  // sequence point: }
  IL_010c:  ldarg.0
  IL_010d:  ldc.i4.s   -2
  IL_010f:  stfld      ""int C.<M>d__0.<>1__state""
  // sequence point: <hidden>
  IL_0114:  ldarg.0
  IL_0115:  ldfld      ""bool C.<M>d__0.<>w__promiseIsActive""
  IL_011a:  brtrue.s   IL_012f
  IL_011c:  ldarg.0
  IL_011d:  ldc.i4.1
  IL_011e:  stfld      ""bool C.<M>d__0.<>w__promiseIsActive""
  IL_0123:  ldarg.0
  IL_0124:  ldflda     ""System.Threading.Tasks.ManualResetValueTaskSourceLogic<bool> C.<M>d__0.<>v__promiseOfValueOrEnd""
  IL_0129:  call       ""void System.Threading.Tasks.ManualResetValueTaskSourceLogic<bool>.Reset()""
  IL_012e:  nop
  IL_012f:  ldarg.0
  IL_0130:  ldflda     ""System.Threading.Tasks.ManualResetValueTaskSourceLogic<bool> C.<M>d__0.<>v__promiseOfValueOrEnd""
  IL_0135:  ldc.i4.0
  IL_0136:  call       ""void System.Threading.Tasks.ManualResetValueTaskSourceLogic<bool>.SetResult(bool)""
  IL_013b:  nop
  IL_013c:  ret
}", sequencePoints: "C+<M>d__0.MoveNext", source: source);
                }
                else
                {
                    verifier.VerifyIL("C.<M>d__0.System.Runtime.CompilerServices.IAsyncStateMachine.MoveNext()", @"
{
  // Code size      298 (0x12a)
  .maxstack  3
  .locals init (int V_0,
                System.Runtime.CompilerServices.TaskAwaiter V_1,
                C.<M>d__0 V_2,
                System.Exception V_3)
  // sequence point: <hidden>
  IL_0000:  ldarg.0
  IL_0001:  ldfld      ""int C.<M>d__0.<>1__state""
  IL_0006:  stloc.0
  .try
  {
    // sequence point: <hidden>
    IL_0007:  ldloc.0
    IL_0008:  brfalse.s  IL_006f
    IL_000a:  ldloc.0
    IL_000b:  ldc.i4.1
    IL_000c:  beq        IL_00c0
    // sequence point: Write(""1 "");
    IL_0011:  ldstr      ""1 ""
    IL_0016:  call       ""void System.Console.Write(string)""
    // sequence point: await System.Threading.Tasks.Task.CompletedTask;
    IL_001b:  call       ""System.Threading.Tasks.Task System.Threading.Tasks.Task.CompletedTask.get""
    IL_0020:  callvirt   ""System.Runtime.CompilerServices.TaskAwaiter System.Threading.Tasks.Task.GetAwaiter()""
    IL_0025:  stloc.1
    // sequence point: <hidden>
    IL_0026:  ldloca.s   V_1
    IL_0028:  call       ""bool System.Runtime.CompilerServices.TaskAwaiter.IsCompleted.get""
    IL_002d:  brtrue.s   IL_008b
    IL_002f:  ldarg.0
    IL_0030:  ldc.i4.0
    IL_0031:  dup
    IL_0032:  stloc.0
    IL_0033:  stfld      ""int C.<M>d__0.<>1__state""
    // async: yield
    IL_0038:  ldarg.0
    IL_0039:  ldloc.1
    IL_003a:  stfld      ""System.Runtime.CompilerServices.TaskAwaiter C.<M>d__0.<>u__1""
    IL_003f:  ldarg.0
    IL_0040:  ldfld      ""bool C.<M>d__0.<>w__promiseIsActive""
    IL_0045:  brtrue.s   IL_0059
    IL_0047:  ldarg.0
    IL_0048:  ldc.i4.1
    IL_0049:  stfld      ""bool C.<M>d__0.<>w__promiseIsActive""
    IL_004e:  ldarg.0
    IL_004f:  ldflda     ""System.Threading.Tasks.ManualResetValueTaskSourceLogic<bool> C.<M>d__0.<>v__promiseOfValueOrEnd""
    IL_0054:  call       ""void System.Threading.Tasks.ManualResetValueTaskSourceLogic<bool>.Reset()""
    IL_0059:  ldarg.0
    IL_005a:  stloc.2
    IL_005b:  ldarg.0
    IL_005c:  ldflda     ""System.Runtime.CompilerServices.AsyncVoidMethodBuilder C.<M>d__0.<>t__builder""
    IL_0061:  ldloca.s   V_1
    IL_0063:  ldloca.s   V_2
    IL_0065:  call       ""void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter, C.<M>d__0>(ref System.Runtime.CompilerServices.TaskAwaiter, ref C.<M>d__0)""
    IL_006a:  leave      IL_0129
    // async: resume
    IL_006f:  ldarg.0
    IL_0070:  ldfld      ""System.Runtime.CompilerServices.TaskAwaiter C.<M>d__0.<>u__1""
    IL_0075:  stloc.1
    IL_0076:  ldarg.0
    IL_0077:  ldflda     ""System.Runtime.CompilerServices.TaskAwaiter C.<M>d__0.<>u__1""
    IL_007c:  initobj    ""System.Runtime.CompilerServices.TaskAwaiter""
    IL_0082:  ldarg.0
    IL_0083:  ldc.i4.m1
    IL_0084:  dup
    IL_0085:  stloc.0
    IL_0086:  stfld      ""int C.<M>d__0.<>1__state""
    IL_008b:  ldloca.s   V_1
    IL_008d:  call       ""void System.Runtime.CompilerServices.TaskAwaiter.GetResult()""
    // sequence point: Write(""2 "");
    IL_0092:  ldstr      ""2 ""
    IL_0097:  call       ""void System.Console.Write(string)""
    // sequence point: yield return 3;
    IL_009c:  ldarg.0
    IL_009d:  ldc.i4.3
    IL_009e:  stfld      ""int C.<M>d__0.<>2__current""
    IL_00a3:  ldarg.0
    IL_00a4:  ldc.i4.1
    IL_00a5:  stfld      ""int C.<M>d__0.<>1__state""
    IL_00aa:  ldarg.0
    IL_00ab:  ldfld      ""bool C.<M>d__0.<>w__promiseIsActive""
    IL_00b0:  brfalse.s  IL_00be
    IL_00b2:  ldarg.0
    IL_00b3:  ldflda     ""System.Threading.Tasks.ManualResetValueTaskSourceLogic<bool> C.<M>d__0.<>v__promiseOfValueOrEnd""
    IL_00b8:  ldc.i4.1
    IL_00b9:  call       ""void System.Threading.Tasks.ManualResetValueTaskSourceLogic<bool>.SetResult(bool)""
    IL_00be:  leave.s    IL_0129
    // sequence point: Write("" 4 "");
    IL_00c0:  ldstr      "" 4 ""
    IL_00c5:  call       ""void System.Console.Write(string)""
    IL_00ca:  leave.s    IL_00fb
  }
  filter
  {
    // sequence point: <hidden>
    IL_00cc:  isinst     ""System.Exception""
    IL_00d1:  dup
    IL_00d2:  brtrue.s   IL_00d8
    IL_00d4:  pop
    IL_00d5:  ldc.i4.0
    IL_00d6:  br.s       IL_00ea
    IL_00d8:  stloc.3
    IL_00d9:  ldarg.0
    IL_00da:  ldc.i4.s   -2
    IL_00dc:  stfld      ""int C.<M>d__0.<>1__state""
    IL_00e1:  ldarg.0
    IL_00e2:  ldfld      ""bool C.<M>d__0.<>w__promiseIsActive""
    IL_00e7:  ldc.i4.0
    IL_00e8:  cgt.un
    IL_00ea:  endfilter
  }  // end filter
  {  // handler
    // sequence point: <hidden>
    IL_00ec:  pop
    IL_00ed:  ldarg.0
    IL_00ee:  ldflda     ""System.Threading.Tasks.ManualResetValueTaskSourceLogic<bool> C.<M>d__0.<>v__promiseOfValueOrEnd""
    IL_00f3:  ldloc.3
    IL_00f4:  call       ""void System.Threading.Tasks.ManualResetValueTaskSourceLogic<bool>.SetException(System.Exception)""
    IL_00f9:  leave.s    IL_0129
  }
  // sequence point: }
  IL_00fb:  ldarg.0
  IL_00fc:  ldc.i4.s   -2
  IL_00fe:  stfld      ""int C.<M>d__0.<>1__state""
  // sequence point: <hidden>
  IL_0103:  ldarg.0
  IL_0104:  ldfld      ""bool C.<M>d__0.<>w__promiseIsActive""
  IL_0109:  brtrue.s   IL_011d
  IL_010b:  ldarg.0
  IL_010c:  ldc.i4.1
  IL_010d:  stfld      ""bool C.<M>d__0.<>w__promiseIsActive""
  IL_0112:  ldarg.0
  IL_0113:  ldflda     ""System.Threading.Tasks.ManualResetValueTaskSourceLogic<bool> C.<M>d__0.<>v__promiseOfValueOrEnd""
  IL_0118:  call       ""void System.Threading.Tasks.ManualResetValueTaskSourceLogic<bool>.Reset()""
  IL_011d:  ldarg.0
  IL_011e:  ldflda     ""System.Threading.Tasks.ManualResetValueTaskSourceLogic<bool> C.<M>d__0.<>v__promiseOfValueOrEnd""
  IL_0123:  ldc.i4.0
  IL_0124:  call       ""void System.Threading.Tasks.ManualResetValueTaskSourceLogic<bool>.SetResult(bool)""
  IL_0129:  ret
}", sequencePoints: "C+<M>d__0.MoveNext", source: source);
                }
            }
        }

        [Fact]
        public void AsyncIteratorWithReturn()
        {
            string source = @"
class C
{
    public static async System.Collections.Generic.IAsyncEnumerable<int> M()
    {
        await System.Threading.Tasks.Task.CompletedTask;
        yield return 3;
        return null;
    }
}";
            var comp = CreateCompilationWithTasksExtensions(new[] { source, s_common });
            comp.VerifyDiagnostics(
                // (8,9): error CS1622: Cannot return a value from an iterator. Use the yield return statement to return a value, or yield break to end the iteration.
                //         return null;
                Diagnostic(ErrorCode.ERR_ReturnInIterator, "return").WithLocation(8, 9)
                );
        }

        [Fact]
        public void AsyncIteratorWithAwaitCompletedAndOneYieldAndOneInvocation()
        {
            string source = @"
using static System.Console;
class C
{
    static async System.Collections.Generic.IAsyncEnumerable<int> M()
    {
        Write(""1 "");
        await System.Threading.Tasks.Task.CompletedTask;
        Write(""2 "");
        yield return 3;
        Write(""4 "");
    }
    static async System.Threading.Tasks.Task Main()
    {
        Write(""0 "");
        foreach await (var i in M())
        {
            Write($""{i} "");
        }
        Write(""Done"");
    }
}";
            var comp = CreateCompilationWithTasksExtensions(new[] { source, s_common }, options: TestOptions.DebugExe);
            comp.VerifyDiagnostics();
            CompileAndVerify(comp, expectedOutput: "0 1 2 3 4 Done");
        }

        [Fact]
        public void AsyncIteratorWithAwaitCompletedAndTwoYields()
        {
            string source = @"
using static System.Console;
class C
{
    static async System.Collections.Generic.IAsyncEnumerable<int> M()
    {
        Write(""1 "");
        await System.Threading.Tasks.Task.CompletedTask;
        Write(""2 "");
        yield return 3;
        Write(""4 "");
        yield return 5;
    }
    static async System.Threading.Tasks.Task Main()
    {
        Write(""0 "");
        foreach await (var i in M())
        {
            Write($""{i} "");
        }
        Write(""Done"");
    }
}";
            var comp = CreateCompilationWithTasksExtensions(new[] { source, s_common }, options: TestOptions.DebugExe);
            comp.VerifyDiagnostics();
            CompileAndVerify(comp, expectedOutput: "0 1 2 3 4 5 Done");
        }

        [Fact]
        public void AsyncIteratorWithYieldAndAwait()
        {
            string source = @"
using static System.Console;
class C
{
    static async System.Collections.Generic.IAsyncEnumerable<int> M()
    {
        Write(""1 "");
        yield return 2;
        Write(""3 "");
        await System.Threading.Tasks.Task.Delay(10);
    }
    static async System.Threading.Tasks.Task Main()
    {
        Write(""0 "");
        foreach await (var i in M())
        {
            Write($""{i} "");
        }
        Write(""Done"");
    }
}";
            var comp = CreateCompilationWithTasksExtensions(new[] { source, s_common }, options: TestOptions.DebugExe);
            comp.VerifyDiagnostics();
            CompileAndVerify(comp, expectedOutput: "0 1 2 3 Done");
        }

        [Fact]
        public void AsyncIteratorWithAwaitCompletedAndYieldBreak()
        {
            string source = @"
using static System.Console;
class C
{
    static async System.Collections.Generic.IAsyncEnumerable<int> M()
    {
        Write(""1 "");
        await System.Threading.Tasks.Task.CompletedTask;
        Write(""2 "");
        yield break;
    }
    static async System.Threading.Tasks.Task Main()
    {
        Write(""0 "");
        foreach await (var i in M())
        {
            Write($""{i} "");
        }
        Write(""Done"");
    }
}";
            var comp = CreateCompilationWithTasksExtensions(new[] { source, s_common }, options: TestOptions.DebugExe);
            comp.VerifyDiagnostics();
            CompileAndVerify(comp, expectedOutput: "0 1 2 Done");
        }

        [Fact]
        public void AsyncIteratorWithAwaitCompletedAndYieldBreakAndYieldReturn()
        {
            string source = @"
using static System.Console;
class C
{
    static async System.Collections.Generic.IAsyncEnumerable<int> M()
    {
        Write(""1 "");
        await System.Threading.Tasks.Task.CompletedTask;
        Write(""2 "");
        goto label2;
label1:
        yield break;
label2:
        yield return 3;
        goto label1;
    }
    static async System.Threading.Tasks.Task Main()
    {
        Write(""0 "");
        foreach await (var i in M())
        {
            Write($""{i} "");
        }
        Write(""Done"");
    }
}";
            var comp = CreateCompilationWithTasksExtensions(new[] { source, s_common }, options: TestOptions.DebugExe);
            comp.VerifyDiagnostics();
            CompileAndVerify(comp, expectedOutput: "0 1 2 3 Done");
        }

        [Fact]
        public void AsyncIteratorWithCustomCode()
        {
            verify(new[] { AwaitSlow, Write, Yield, AwaitSlow });
            verify(new[] { AwaitSlow, Write, Yield, Yield });
            verify(new[] { Write, Yield, Write, AwaitFast, Yield });
            verify(new[] { Yield, Write, AwaitFast, Yield });
            verify(new[] { AwaitFast, YieldBreak });
            verify(new[] { AwaitSlow, YieldBreak });
            verify(new[] { AwaitSlow, Yield, YieldBreak });

            void verify(Instruction[] spec)
            {
                verifyMethod(spec);
                verifyLocalFunction(spec);
            }

            void verifyMethod(Instruction[] spec)
            {
                (string code, string expectation) = generateCode(spec);

                string source = $@"
using static System.Console;
class C
{{
    static async System.Collections.Generic.IAsyncEnumerable<int> M()
    {{
        {code}
    }}
    static async System.Threading.Tasks.Task Main()
    {{
        Write(""0 "");
        foreach await (var i in M())
        {{
            Write($""{{i}} "");
        }}
        Write(""Done"");
    }}
}}";
                var comp = CreateCompilationWithTasksExtensions(new[] { source, s_common }, options: TestOptions.DebugExe);
                comp.VerifyDiagnostics();
                var verifier = CompileAndVerify(comp, expectedOutput: expectation);
            }

            void verifyLocalFunction(Instruction[] spec)
            {
                (string code, string expectation) = generateCode(spec);

                string source = $@"
using static System.Console;
class C
{{
    static async System.Threading.Tasks.Task Main()
    {{
        Write(""0 "");
        foreach await (var i in local())
        {{
            Write($""{{i}} "");
        }}
        Write(""Done"");

        async System.Collections.Generic.IAsyncEnumerable<int> local()
        {{
            {code}
        }}
    }}
}}";
                var comp = CreateCompilationWithTasksExtensions(new[] { source, s_common }, options: TestOptions.DebugExe);
                comp.VerifyDiagnostics();
                var verifier = CompileAndVerify(comp, expectedOutput: expectation);
            }

            (string code, string expectation) generateCode(Instruction[] spec)
            {
                var builder = new StringBuilder();
                var expectationBuilder = new StringBuilder();
                int counter = 1;
                expectationBuilder.Append("0 ");

                foreach (var instruction in spec)
                {
                    switch (instruction)
                    {
                        case Write:
                            //Write(""N "");
                            builder.AppendLine($@"Write(""{counter} "");");
                            expectationBuilder.Append($"{counter} ");
                            counter++;
                            break;
                        case Yield:
                            //yield return N;
                            builder.AppendLine($@"yield return {counter};");
                            expectationBuilder.Append($"{counter} ");
                            counter++;
                            break;
                        case AwaitSlow:
                            //await System.Threading.Tasks.Task.Delay(10);
                            builder.AppendLine("await System.Threading.Tasks.Task.Delay(10);");
                            break;
                        case AwaitFast:
                            //await new System.Threading.Tasks.Task.CompletedTask;
                            builder.AppendLine("await System.Threading.Tasks.Task.CompletedTask;");
                            break;
                        case YieldBreak:
                            //yield break;
                            builder.AppendLine($@"yield break;");
                            break;
                    }
                }
                expectationBuilder.Append("Done");
                return (builder.ToString(), expectationBuilder.ToString());
            }
        }

        [Fact]
        public void AsyncIteratorWithAwaitAndYieldAndAwait()
        {
            string source = @"
using static System.Console;
class C
{
    static async System.Collections.Generic.IAsyncEnumerable<int> M()
    {
        Write(""1 "");
        await System.Threading.Tasks.Task.Delay(10);
        Write(""2 "");
        yield return 3;
        Write(""4 "");
        await System.Threading.Tasks.Task.Delay(10);
    }
    static async System.Threading.Tasks.Task Main()
    {
        Write(""0 "");
        foreach await (var i in M())
        {
            Write($""{i} "");
        }
        Write(""Done"");
    }
}";
            var comp = CreateCompilationWithTasksExtensions(new[] { source, s_common }, options: TestOptions.DebugExe);
            comp.VerifyDiagnostics();
            var verifier = CompileAndVerify(comp, expectedOutput: "0 1 2 3 4 Done");
        }

        [Fact]
        public void AsyncIteratorWithAwaitOnly()
        {
            string source = @"
class C
{
    async System.Collections.Generic.IAsyncEnumerable<int> M()
    {
        await System.Threading.Tasks.Task.CompletedTask;
    }
}";
            var comp = CreateCompilationWithTasksExtensions(new[] { source, s_common });
            comp.VerifyDiagnostics(
                // (4,60): error CS0161: 'C.M()': not all code paths return a value
                //     async System.Collections.Generic.IAsyncEnumerable<int> M()
                Diagnostic(ErrorCode.ERR_ReturnExpected, "M").WithArguments("C.M()").WithLocation(4, 60)
                );
        }

        [Fact]
        public void AsyncIteratorWithYieldReturnOnly()
        {
            string source = @"
class C
{
    static async System.Collections.Generic.IAsyncEnumerable<int> M()
    {
        yield return 1;
    }
    public static async System.Threading.Tasks.Task Main()
    {
        foreach await (var i in M())
        {
            System.Console.Write(i);
        }
    }
}";
            var comp = CreateCompilationWithTasksExtensions(new[] { source, s_common }, options: TestOptions.DebugExe);
            comp.VerifyDiagnostics(
                // (4,67): warning CS1998: This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
                //     static async System.Collections.Generic.IAsyncEnumerable<int> M()
                Diagnostic(ErrorCode.WRN_AsyncLacksAwaits, "M").WithLocation(4, 67)
                );
            CompileAndVerify(comp, expectedOutput: "1");
        }

        [Fact]
        public void AsyncIteratorWithYieldBreakOnly()
        {
            string source = @"
class C
{
    static async System.Collections.Generic.IAsyncEnumerable<int> M()
    {
        yield break;
    }
    public static async System.Threading.Tasks.Task Main()
    {
        foreach await (var i in M())
        {
            System.Console.Write(""SKIPPED"");
        }
        System.Console.Write(""none"");
    }
}";
            var comp = CreateCompilationWithTasksExtensions(new[] { source, s_common }, options: TestOptions.DebugExe);
            comp.VerifyDiagnostics(
                // (4,67): warning CS1998: This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
                //     static async System.Collections.Generic.IAsyncEnumerable<int> M()
                Diagnostic(ErrorCode.WRN_AsyncLacksAwaits, "M").WithLocation(4, 67)
                );
            CompileAndVerify(comp, expectedOutput: "none");
        }

        [Fact]
        public void AsyncIteratorWithoutAwaitOrYield()
        {
            string source = @"
class C
{
    async System.Collections.Generic.IAsyncEnumerable<int> M()
    {
    }
}";
            var comp = CreateCompilationWithTasksExtensions(new[] { source, s_common });
            comp.VerifyDiagnostics(
                // (4,60): warning CS1998: This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
                //     async System.Collections.Generic.IAsyncEnumerable<int> M()
                Diagnostic(ErrorCode.WRN_AsyncLacksAwaits, "M").WithLocation(4, 60),
                // (4,60): error CS0161: 'C.M()': not all code paths return a value
                //     async System.Collections.Generic.IAsyncEnumerable<int> M()
                Diagnostic(ErrorCode.ERR_ReturnExpected, "M").WithArguments("C.M()").WithLocation(4, 60)
                );
        }

        [Fact]
        public void TestBadReturnValue()
        {
            string source = @"
class C
{
    async System.Collections.Generic.IAsyncEnumerable<int> M()
    {
        yield return ""hello"";
        yield return;
    }
}";
            var comp = CreateCompilationWithTasksExtensions(new[] { source, s_common });
            comp.VerifyDiagnostics(
                // (7,15): error CS1627: Expression expected after yield return
                //         yield return;
                Diagnostic(ErrorCode.ERR_EmptyYield, "return").WithLocation(7, 15),
                // (6,22): error CS0029: Cannot implicitly convert type 'string' to 'int'
                //         yield return "hello";
                Diagnostic(ErrorCode.ERR_NoImplicitConv, @"""hello""").WithArguments("string", "int").WithLocation(6, 22),
                // (4,60): warning CS1998: This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
                //     async System.Collections.Generic.IAsyncEnumerable<int> M()
                Diagnostic(ErrorCode.WRN_AsyncLacksAwaits, "M").WithLocation(4, 60)
                );
        }

        [Fact]
        public void TestWaitForNextAsyncCalledOutOfTurn()
        {
            string source = @"
using static System.Console;
class C
{
    public static async System.Threading.Tasks.Task Main()
    {
        using await (var enumerator = new C().M().GetAsyncEnumerator())
        {
            await enumerator.WaitForNextAsync();
            await enumerator.WaitForNextAsync();
            await enumerator.WaitForNextAsync();

            var value = enumerator.TryGetNext(out bool success);
            Assert(success);
            Assert(value == 42);

            enumerator.TryGetNext(out success);
            Assert(!success);

            await enumerator.WaitForNextAsync();
            await enumerator.WaitForNextAsync();
            await enumerator.WaitForNextAsync();

            value = enumerator.TryGetNext(out success);
            Assert(success);
            Assert(value == 43);
        }
        Write(""Done"");
    }
    async System.Collections.Generic.IAsyncEnumerable<int> M()
    {
        yield return 42;
        await new System.Threading.Tasks.ValueTask(System.Threading.Tasks.Task.Delay(100));
        yield return 43;
    }
    static void Assert(bool b)
    {
        if (!b) throw null;
    }
}";
            var comp = CreateCompilationWithTasksExtensions(new[] { source, s_common }, options: TestOptions.DebugExe);
            comp.VerifyDiagnostics();
            CompileAndVerify(comp, expectedOutput: "Done");
        }

        [Fact]
        public void TestTryGetNextCalledOutOfTurn()
        {
            string source = @"
using static System.Console;
class C
{
    public static async System.Threading.Tasks.Task Main()
    {
        using await (var enumerator = new C().M().GetAsyncEnumerator())
        {
            await enumerator.WaitForNextAsync();

            var value = enumerator.TryGetNext(out bool success);
            Assert(success);
            Assert(value == 42);

            enumerator.TryGetNext(out success);
            Assert(!success);

            try
            {
                enumerator.TryGetNext(out success);
            }
            catch (System.Exception e)
            {
                Assert(e != null);
                Write(""Done"");
            }
        }
    }
    async System.Collections.Generic.IAsyncEnumerable<int> M()
    {
        yield return 42;
        await new System.Threading.Tasks.ValueTask(System.Threading.Tasks.Task.Delay(100));
        yield return 43;
    }
    static void Assert(bool b)
    {
        if (!b) throw null;
    }
}";
            var comp = CreateCompilationWithTasksExtensions(new[] { source, s_common }, options: TestOptions.DebugExe);
            comp.VerifyDiagnostics();
            // PROTOTYPE(async-streams): need to implement the exception
            //CompileAndVerify(comp, expectedOutput: "Done");
        }

        [Fact]
        public void TestThrownException_WhilePromiseInactive()
        {
            string source = @"
using static System.Console;
class C
{
    public static async System.Threading.Tasks.Task Main()
    {
        using await (var enumerator = new C().M().GetAsyncEnumerator())
        {
            await enumerator.WaitForNextAsync();

            var value = enumerator.TryGetNext(out bool success);
            Assert(success);
            Assert(value == 42);

            try
            {
                enumerator.TryGetNext(out success);
                Write(""UNREACHABLE"");
            }
            catch (System.Exception e)
            {
                Assert(e.Message == ""message"");
            }
            Write(""Done"");
        }
    }
    async System.Collections.Generic.IAsyncEnumerable<int> M()
    {
        await System.Threading.Tasks.Task.CompletedTask;
        yield return 42;
        bool b = true;
        if (b) throw new System.Exception(""message"");
        Write(""UNREACHABLE2"");
    }
    static void Assert(bool b)
    {
        if (!b) throw null;
    }
}";
            var comp = CreateCompilationWithTasksExtensions(new[] { source, s_common }, options: TestOptions.DebugExe);
            comp.VerifyDiagnostics();
            CompileAndVerify(comp, expectedOutput: "Done");
        }

        [Fact]
        public void TestThrownException_WhilePromiseActive()
        {
            string source = @"
using static System.Console;
class C
{
    public static async System.Threading.Tasks.Task Main()
    {
        using await (var enumerator = new C().M().GetAsyncEnumerator())
        {
            await enumerator.WaitForNextAsync();

            var value = enumerator.TryGetNext(out bool success);
            Assert(success);
            Assert(value == 42);

            enumerator.TryGetNext(out success);
            Assert(!success);

            try
            {
                await enumerator.WaitForNextAsync();
                Write(""UNREACHABLE"");
            }
            catch (System.Exception e)
            {
                Assert(e.Message == ""message"");
            }
            Write(""Done"");
        }
    }
    async System.Collections.Generic.IAsyncEnumerable<int> M()
    {
        yield return 42;
        await new System.Threading.Tasks.ValueTask(System.Threading.Tasks.Task.Delay(100));
        bool b = true;
        if (b) throw new System.Exception(""message"");
        Write(""UNREACHABLE2"");
    }
    static void Assert(bool b)
    {
        if (!b) throw null;
    }
}";
            var comp = CreateCompilationWithTasksExtensions(new[] { source, s_common }, options: TestOptions.DebugExe);
            comp.VerifyDiagnostics();
            CompileAndVerify(comp, expectedOutput: "Done");
        }

        // PROTOTYPE(async-streams): Consider moving this common test code to TestSources.cs
        private static readonly string s_common = @"
namespace System.Collections.Generic
{
    public interface IAsyncEnumerable<out T>
    {
        IAsyncEnumerator<T> GetAsyncEnumerator();
    }

    public interface IAsyncEnumerator<out T> : System.IAsyncDisposable
    {
        System.Threading.Tasks.ValueTask<bool> WaitForNextAsync();
        T TryGetNext(out bool success);
    }
}
namespace System
{
    public interface IAsyncDisposable
    {
        System.Threading.Tasks.ValueTask DisposeAsync();
    }
}

namespace System.Runtime.CompilerServices
{
    public interface IStrongBox<T>
    {
        ref T Value { get; }
    }
}

namespace System.Threading.Tasks
{
    using System.Runtime.CompilerServices;
    using System.Runtime.ExceptionServices;
    using System.Threading.Tasks.Sources;

    public struct ManualResetValueTaskSourceLogic<TResult>
    {
        private static readonly Action<object> s_sentinel = new Action<object>(s => throw new InvalidOperationException());

        private readonly IStrongBox<ManualResetValueTaskSourceLogic<TResult>> _parent;
        private Action<object> _continuation;
        private object _continuationState;
        private object _capturedContext;
        private ExecutionContext _executionContext;
        private bool _completed;
        private TResult _result;
        private ExceptionDispatchInfo _error;
        private short _version;

        public ManualResetValueTaskSourceLogic(IStrongBox<ManualResetValueTaskSourceLogic<TResult>> parent)
        {
            _parent = parent ?? throw new ArgumentNullException(nameof(parent));
            _continuation = null;
            _continuationState = null;
            _capturedContext = null;
            _executionContext = null;
            _completed = false;
            _result = default;
            _error = null;
            _version = 0;
        }

        public short Version => _version;

        private void ValidateToken(short token)
        {
            if (token != _version)
            {
                throw new InvalidOperationException();
            }
        }

        public ValueTaskSourceStatus GetStatus(short token)
        {
            ValidateToken(token);

            return
                !_completed ? ValueTaskSourceStatus.Pending :
                _error == null ? ValueTaskSourceStatus.Succeeded :
                _error.SourceException is OperationCanceledException ? ValueTaskSourceStatus.Canceled :
                ValueTaskSourceStatus.Faulted;
        }

        public TResult GetResult(short token)
        {
            ValidateToken(token);

            if (!_completed)
            {
                throw new InvalidOperationException();
            }

            _error?.Throw();
            return _result;
        }

        public void Reset()
        {
            _version++;

            _completed = false;
            _continuation = null;
            _continuationState = null;
            _result = default;
            _error = null;
            _executionContext = null;
            _capturedContext = null;
        }

        public void OnCompleted(Action<object> continuation, object state, short token, ValueTaskSourceOnCompletedFlags flags)
        {
            if (continuation == null)
            {
                throw new ArgumentNullException(nameof(continuation));
            }
            ValidateToken(token);

            if ((flags & ValueTaskSourceOnCompletedFlags.FlowExecutionContext) != 0)
            {
                _executionContext = ExecutionContext.Capture();
            }

            if ((flags & ValueTaskSourceOnCompletedFlags.UseSchedulingContext) != 0)
            {
                SynchronizationContext sc = SynchronizationContext.Current;
                if (sc != null && sc.GetType() != typeof(SynchronizationContext))
                {
                    _capturedContext = sc;
                }
                else
                {
                    TaskScheduler ts = TaskScheduler.Current;
                    if (ts != TaskScheduler.Default)
                    {
                        _capturedContext = ts;
                    }
                }
            }

            _continuationState = state;
            if (Interlocked.CompareExchange(ref _continuation, continuation, null) != null)
            {
                _executionContext = null;

                object cc = _capturedContext;
                _capturedContext = null;

                switch (cc)
                {
                    case null:
                        Task.Factory.StartNew(continuation, state, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
                        break;

                    case SynchronizationContext sc:
                        sc.Post(s =>
                        {
                            var tuple = (Tuple<Action<object>, object>)s;
                            tuple.Item1(tuple.Item2);
                        }, Tuple.Create(continuation, state));
                        break;

                    case TaskScheduler ts:
                        Task.Factory.StartNew(continuation, state, CancellationToken.None, TaskCreationOptions.DenyChildAttach, ts);
                        break;
                }
            }
        }

        public void SetResult(TResult result)
        {
            _result = result;
            SignalCompletion();
        }

        public void SetException(Exception error)
        {
            _error = ExceptionDispatchInfo.Capture(error);
            SignalCompletion();
        }

        private void SignalCompletion()
        {
            if (_completed)
            {
                throw new InvalidOperationException();
            }
            _completed = true;

            if (Interlocked.CompareExchange(ref _continuation, s_sentinel, null) != null)
            {
                if (_executionContext != null)
                {
                    ExecutionContext.Run(
                        _executionContext,
                        s => ((IStrongBox<ManualResetValueTaskSourceLogic<TResult>>)s).Value.InvokeContinuation(),
                        _parent ?? throw new InvalidOperationException());
                }
                else
                {
                    InvokeContinuation();
                }
            }
        }

        private void InvokeContinuation()
        {
            object cc = _capturedContext;
            _capturedContext = null;

            switch (cc)
            {
                case null:
                    _continuation(_continuationState);
                    break;

                case SynchronizationContext sc:
                    sc.Post(s =>
                    {
                        ref ManualResetValueTaskSourceLogic<TResult> logicRef = ref ((IStrongBox<ManualResetValueTaskSourceLogic<TResult>>)s).Value;
                        logicRef._continuation(logicRef._continuationState);
                    }, _parent ?? throw new InvalidOperationException());
                    break;

                case TaskScheduler ts:
                    Task.Factory.StartNew(_continuation, _continuationState, CancellationToken.None, TaskCreationOptions.DenyChildAttach, ts);
                    break;
            }
        }
    }
}
";

        [Fact]
        public void TestWellKnownMembers()
        {
            var comp = CreateCompilation(s_common, references: new[] { TestReferences.NetStandard20.TasksExtensionsRef }, targetFramework: Roslyn.Test.Utilities.TargetFramework.NetStandard20);
            comp.VerifyDiagnostics();

            verifyType(WellKnownType.System_Runtime_CompilerServices_IStrongBox_T,
                "System.Runtime.CompilerServices.IStrongBox<T>");

            verifyMember(WellKnownMember.System_Runtime_CompilerServices_IStrongBox_T__Value,
                "ref T System.Runtime.CompilerServices.IStrongBox<T>.Value { get; }");

            verifyMember(WellKnownMember.System_Runtime_CompilerServices_IStrongBox_T__get_Value,
                "ref T System.Runtime.CompilerServices.IStrongBox<T>.Value.get");

            verifyType(WellKnownType.System_Threading_Tasks_ManualResetValueTaskSourceLogic_T,
                "System.Threading.Tasks.ManualResetValueTaskSourceLogic<TResult>");

            verifyMember(WellKnownMember.System_Threading_Tasks_ManualResetValueTaskSourceLogic_T__ctor,
                "System.Threading.Tasks.ManualResetValueTaskSourceLogic<TResult>..ctor(System.Runtime.CompilerServices.IStrongBox<System.Threading.Tasks.ManualResetValueTaskSourceLogic<TResult>> parent)");

            verifyMember(WellKnownMember.System_Threading_Tasks_ManualResetValueTaskSourceLogic_T__GetResult,
                "TResult System.Threading.Tasks.ManualResetValueTaskSourceLogic<TResult>.GetResult(System.Int16 token)");

            verifyMember(WellKnownMember.System_Threading_Tasks_ManualResetValueTaskSourceLogic_T__GetStatus,
                "System.Threading.Tasks.Sources.ValueTaskSourceStatus System.Threading.Tasks.ManualResetValueTaskSourceLogic<TResult>.GetStatus(System.Int16 token)");

            verifyMember(WellKnownMember.System_Threading_Tasks_ManualResetValueTaskSourceLogic_T__OnCompleted,
                "void System.Threading.Tasks.ManualResetValueTaskSourceLogic<TResult>.OnCompleted(System.Action<System.Object> continuation, System.Object state, System.Int16 token, System.Threading.Tasks.Sources.ValueTaskSourceOnCompletedFlags flags)");

            verifyMember(WellKnownMember.System_Threading_Tasks_ManualResetValueTaskSourceLogic_T__Reset,
                "void System.Threading.Tasks.ManualResetValueTaskSourceLogic<TResult>.Reset()");

            verifyMember(WellKnownMember.System_Threading_Tasks_ManualResetValueTaskSourceLogic_T__SetException,
                "void System.Threading.Tasks.ManualResetValueTaskSourceLogic<TResult>.SetException(System.Exception error)");

            verifyMember(WellKnownMember.System_Threading_Tasks_ManualResetValueTaskSourceLogic_T__SetResult,
                "void System.Threading.Tasks.ManualResetValueTaskSourceLogic<TResult>.SetResult(TResult result)");

            verifyMember(WellKnownMember.System_Threading_Tasks_ManualResetValueTaskSourceLogic_T__get_Version,
                "System.Int16 System.Threading.Tasks.ManualResetValueTaskSourceLogic<TResult>.Version.get");

            verifyType(WellKnownType.System_Threading_Tasks_Sources_ValueTaskSourceStatus,
                "System.Threading.Tasks.Sources.ValueTaskSourceStatus");

            verifyType(WellKnownType.System_Threading_Tasks_Sources_ValueTaskSourceOnCompletedFlags,
                "System.Threading.Tasks.Sources.ValueTaskSourceOnCompletedFlags");

            verifyType(WellKnownType.System_Threading_Tasks_Sources_IValueTaskSource_T,
                "System.Threading.Tasks.Sources.IValueTaskSource<out TResult>");

            verifyMember(WellKnownMember.System_Threading_Tasks_Sources_IValueTaskSource_T__GetResult,
                "TResult System.Threading.Tasks.Sources.IValueTaskSource<out TResult>.GetResult(System.Int16 token)");

            verifyMember(WellKnownMember.System_Threading_Tasks_Sources_IValueTaskSource_T__GetStatus,
                "System.Threading.Tasks.Sources.ValueTaskSourceStatus System.Threading.Tasks.Sources.IValueTaskSource<out TResult>.GetStatus(System.Int16 token)");

            verifyMember(WellKnownMember.System_Threading_Tasks_Sources_IValueTaskSource_T__OnCompleted,
                "void System.Threading.Tasks.Sources.IValueTaskSource<out TResult>.OnCompleted(System.Action<System.Object> continuation, System.Object state, System.Int16 token, System.Threading.Tasks.Sources.ValueTaskSourceOnCompletedFlags flags)");

            verifyType(WellKnownType.System_Threading_Tasks_ValueTask_T,
                "System.Threading.Tasks.ValueTask<TResult>");

            verifyMember(WellKnownMember.System_Threading_Tasks_ValueTask_T__ctor,
                "System.Threading.Tasks.ValueTask<TResult>..ctor(System.Threading.Tasks.Sources.IValueTaskSource<TResult> source, System.Int16 token)");

            void verifyType(WellKnownType type, string expected)
            {
                var symbol = comp.GetWellKnownType(type);
                Assert.Equal(expected, symbol.ToTestDisplayString());
            }

            void verifyMember(WellKnownMember member, string expected)
            {
                var symbol = comp.GetWellKnownTypeMember(member);
                Assert.Equal(expected, symbol.ToTestDisplayString());
            }
        }

        [Fact]
        public void AsyncLocalFunctionWithUnknownReturnType()
        {
            string source = @"
class C
{
    void Method()
    {
        _ = local();

        async Unknown local()
        {
            await System.Threading.Tasks.Task.CompletedTask;
            yield return 3;
        }
    }
}
";
            var comp = CreateCompilationWithTasksExtensions(new[] { source });
            comp.VerifyDiagnostics(
                // (8,15): error CS0246: The type or namespace name 'Unknown' could not be found (are you missing a using directive or an assembly reference?)
                //         async Unknown local()
                Diagnostic(ErrorCode.ERR_SingleTypeNameNotFound, "Unknown").WithArguments("Unknown").WithLocation(8, 15),
                // (8,23): error CS1983: The return type of an async method must be void, Task, Task<T>, a task-like type, or IAsyncEnumerable<T>
                //         async Unknown local()
                Diagnostic(ErrorCode.ERR_BadAsyncReturn, "local").WithLocation(8, 23)
                );
        }
    }
}