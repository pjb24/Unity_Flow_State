// Per-invocation scope contract for the embedded Cloud Code runtime.
// It deliberately has no environment constants and no mutable process state.
function createExecutionScope(context) {
  if (!context || typeof context.projectId !== "string" || context.projectId.trim().length === 0 ||
      context.projectId !== context.projectId.trim() ||
      typeof context.environmentId !== "string" || context.environmentId.trim().length === 0 ||
      context.environmentId !== context.environmentId.trim())
    return null;
  return Object.freeze({ projectId: context.projectId, environmentId: context.environmentId });
}

function matchesExecutionScope(value, scope) {
  return !!value && !!createExecutionScope(scope) && value.projectId === scope.projectId && value.environmentId === scope.environmentId;
}

function createScopedValue(scope, value) {
  if (!createExecutionScope(scope) || !value || typeof value !== "object") return null;
  return { ...value, projectId: scope.projectId, environmentId: scope.environmentId };
}

module.exports = { createExecutionScope, matchesExecutionScope, createScopedValue };
