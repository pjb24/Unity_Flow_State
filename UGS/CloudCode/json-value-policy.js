// Service JSON object member order is not data. Array order and every value are data.
function sameJsonValue(left, right) {
  if (left === null || right === null) return left === right;
  if (typeof left !== typeof right) return false;
  if (typeof left === "number") return Number.isFinite(left) && Number.isFinite(right) && left === right;
  if (typeof left === "string" || typeof left === "boolean") return left === right;
  if (typeof left !== "object") return false;
  if (Array.isArray(left) || Array.isArray(right)) {
    if (!Array.isArray(left) || !Array.isArray(right) || left.length !== right.length) return false;
    for (let i = 0; i < left.length; i++) if (!sameJsonValue(left[i], right[i])) return false;
    return true;
  }
  if (Object.prototype.toString.call(left) !== "[object Object]" ||
      Object.prototype.toString.call(right) !== "[object Object]") return false;
  const keys = Object.keys(left);
  if (keys.length !== Object.keys(right).length) return false;
  for (const key of keys)
    if (!Object.prototype.hasOwnProperty.call(right, key) || !sameJsonValue(left[key], right[key])) return false;
  return true;
}
module.exports = { sameJsonValue };
