// Only trusted embedded server code executes here. This is not a player scripting API.
const Buffer = (() => {
  class Bytes extends Array {
    toString(encoding) {
      const hex = Array.from(this, b => b.toString(16).padStart(2, "0")).join("");
      return encoding === "hex" ? hex : __encoding(hex, "hex", encoding);
    }
    readUInt32BE(offset) {
      if (!Number.isInteger(offset) || offset < 0 || offset + 4 > this.length) throw new Error("Invalid offset");
      return this[offset] * 16777216 + this[offset + 1] * 65536 + this[offset + 2] * 256 + this[offset + 3];
    }
  }
  return {
    from(value, encoding) {
      const hex = encoding === "hex" ? value : __encoding(value, encoding, "hex");
      if (typeof hex !== "string" || hex.length % 2 || !/^[a-fA-F0-9]*$/.test(hex)) throw new Error("Invalid bytes");
      const result = new Bytes();
      for (let i = 0; i < hex.length; i += 2) result.push(parseInt(hex.slice(i, i + 2), 16));
      return result;
    },
    isBuffer(value) { return value instanceof Bytes; }
  };
})();
const __crypto = {
  randomBytes(count) { return Buffer.from(__random(count), "hex"); },
  createHmac(algorithm, key) {
    if (algorithm !== "sha256") throw new Error("Unsupported algorithm");
    let input = "";
    return {
      update(text) { input += text; return this; },
      digest() { return Buffer.from(__hmac(key.toString("hex"), input), "hex"); }
    };
  },
  timingSafeEqual(left, right) {
    if (left.length !== right.length) throw new Error("Invalid digest length");
    return __equals(left.toString("hex"), right.toString("hex"));
  }
};
function __unwrap(json) {
  const response = JSON.parse(json);
  if (!response.ok) {
    const error = new Error("ServiceUnavailable");
    error.response = { status: response.status };
    throw error;
  }
  return response.value;
}
function __sdk(methods) {
  function Api() {}
  for (const method of methods) Api.prototype[method] = async function(...args) {
    return __unwrap(await __service(method, JSON.stringify(args)));
  };
  return Api;
}
const __cache = Object.create(null);
function __load(name) {
  if (name === "crypto") return __crypto;
  if (name === "@unity-services/cloud-save-1.4") return { DataApi: __sdk([
    "getPrivateCustomItems", "setPrivateCustomItem", "setPrivateCustomItemBatch",
    "deletePrivateCustomItem",
    "getProtectedItems", "setProtectedItem", "setProtectedItemBatch"]) };
  if (name === "@unity-services/leaderboards-1.1") return { LeaderboardsApi: __sdk([
    "getLeaderboardScores", "getLeaderboardPlayerScore", "getLeaderboardPlayerRange",
    "addLeaderboardPlayerScore"]) };
  name = name.replace(/^\.\//, "").replace(/\.js$/, "");
  if (!/^[a-z0-9-]+$/.test(name)) throw new Error("Unsupported module");
  if (!__cache[name]) {
    const module = { exports: {} };
    __cache[name] = module;
    new Function("require", "module", "exports", __source(name + ".js"))(__load, module, module.exports);
  }
  return __cache[name].exports;
}
async function __invoke(endpoint, contextJson, paramsJson) {
  const result = await __load(endpoint)({ context: JSON.parse(contextJson), params: JSON.parse(paramsJson),
    secretManager: { async getSecret(name) { return __unwrap(await __secret(name)); } } });
  return JSON.stringify(result);
}
