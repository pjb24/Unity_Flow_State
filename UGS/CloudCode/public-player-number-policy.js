// Pure policy only. Allocation and account mapping require a server-side store.
const width = 10;
const minimum = 1;
const maximum = 9999999999;
const digits = /^\d{10}$/;

function tryFormatIssuedNumber(sequence) {
  if (!Number.isSafeInteger(sequence) || sequence < minimum || sequence > maximum)
    return null;
  return String(sequence).padStart(width, "0");
}

function tryNormalizePublicPlayerNumber(value) {
  if (typeof value !== "string" || !digits.test(value)) return null;
  const sequence = Number(value);
  return sequence >= minimum && sequence <= maximum ? value : null;
}

module.exports = {
  width,
  minimum,
  maximum,
  tryFormatIssuedNumber,
  tryNormalizePublicPlayerNumber
};
