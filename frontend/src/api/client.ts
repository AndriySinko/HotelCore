export async function mockResponse<T>(data: T, delayMs = 250): Promise<T> {
  await new Promise((resolve) => setTimeout(resolve, delayMs));
  return data;
}
