using System;
using JetBrains.Annotations;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

// ReSharper disable EmptyGeneralCatchClause
// ReSharper disable PartialTypeWithSinglePart

namespace AwaitableExtensions;

[PublicAPI]
public static partial class AwaitableExtensions {
    public static Action<Exception> ExceptionHandler { get; set; } = Debug.LogException;
    public static bool HandleExceptionOnMainThread { get; set; } = true;

#if UNITY_EDITOR
    [InitializeOnEnterPlayMode]
    static void OnEnterPlayModeInEditor(EnterPlayModeOptions options) {
        ExceptionHandler = Debug.LogException;
        HandleExceptionOnMainThread = true;
    }
#endif

    public static void Forget(this Awaitable task) {
        Forget(task, ExceptionHandler, HandleExceptionOnMainThread);
    }

    public static void Forget(this Awaitable task, Action<Exception> exceptionHandler,
        bool handleExceptionOnMainThread = true) {
        if (handleExceptionOnMainThread) ForgetCore(HandleExceptionOnMainThread(task, exceptionHandler));
        else ForgetCore(task, exceptionHandler);
        return;

        static async Awaitable HandleExceptionOnMainThread(Awaitable task, Action<Exception> exceptionHandler) {
            try {
                await task;
            } catch (Exception e) {
                if (exceptionHandler is null) return;
                try {
                    await Awaitable.MainThreadAsync();
                    exceptionHandler(e);
                } catch { }
            }
        }
    }

    static void ForgetCore(Awaitable task, Action<Exception> exceptionHandler = null) {
        var awaiter = task.GetAwaiter();
        if (awaiter.IsCompleted) GetResult(awaiter, exceptionHandler);
        else awaiter.OnCompleted(() => GetResult(awaiter, exceptionHandler));
        return;

        static void GetResult(Awaitable.Awaiter awaiter, Action<Exception> exceptionHandler) {
            try {
                awaiter.GetResult();
            } catch (Exception e) {
                if (exceptionHandler is null) return;
                try {
                    exceptionHandler(e);
                } catch { }
            }
        }
    }

    public static void Forget<T>(this Awaitable<T> task) {
        Forget(task, ExceptionHandler, HandleExceptionOnMainThread);
    }

    public static void Forget<T>(this Awaitable<T> task, Action<Exception> exceptionHandler,
        bool handleExceptionOnMainThread = true) {
        if (handleExceptionOnMainThread) ForgetCore(HandleExceptionOnMainThread(task, exceptionHandler));
        else ForgetCore(task, exceptionHandler);
        return;

        static async Awaitable HandleExceptionOnMainThread(Awaitable<T> task, Action<Exception> exceptionHandler) {
            try {
                await task;
            } catch (Exception e) {
                if (exceptionHandler is null) return;
                try {
                    await Awaitable.MainThreadAsync();
                    exceptionHandler(e);
                } catch { }
            }
        }
    }

    static void ForgetCore<T>(Awaitable<T> task, Action<Exception> exceptionHandler = null) {
        var awaiter = task.GetAwaiter();
        if (awaiter.IsCompleted) GetResult(awaiter, exceptionHandler);
        else awaiter.OnCompleted(() => GetResult(awaiter, exceptionHandler));
        return;

        static void GetResult(Awaitable<T>.Awaiter awaiter, Action<Exception> exceptionHandler) {
            try {
                awaiter.GetResult();
            } catch (Exception e) {
                if (exceptionHandler is null) return;
                try {
                    exceptionHandler(e);
                } catch { }
            }
        }
    }
}
