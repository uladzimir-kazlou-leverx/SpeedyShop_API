const { performance } = require('node:perf_hooks');

const baseUrl = process.env.BASE_URL || 'http://localhost:5000';
const durationSeconds = Number(process.env.DURATION_SECONDS || 120);
const concurrency = Number(process.env.CONCURRENCY || 25);
const thinkTimeMs = Number(process.env.THINK_TIME_MS || 250);

const endpoints = [
  '/api/products?take=50',
  '/api/products/popular?take=25',
  '/api/dashboard',
  () => `/api/orders/${1 + Math.floor(Math.random() * 1000)}`,
  () => `/api/orders/customer/${1 + Math.floor(Math.random() * 1000)}/history`,
  '/api/products/search?term=Product',
  () => `/api/products/${1 + Math.floor(Math.random() * 1000)}`,
];

const stats = {
  total: 0,
  ok: 0,
  failed: 0,
  durations: [],
  byEndpoint: new Map(),
};

const sleep = (ms) => new Promise((resolve) => setTimeout(resolve, ms));

function pickEndpoint() {
  const endpoint = endpoints[Math.floor(Math.random() * endpoints.length)];
  return typeof endpoint === 'function' ? endpoint() : endpoint;
}

function percentile(values, p) {
  if (values.length === 0) return 0;
  const sorted = [...values].sort((a, b) => a - b);
  const index = Math.min(sorted.length - 1, Math.ceil((p / 100) * sorted.length) - 1);
  return sorted[index];
}

function record(endpoint, status, durationMs, error) {
  stats.total += 1;
  if (status >= 200 && status < 500) stats.ok += 1;
  else stats.failed += 1;
  stats.durations.push(durationMs);

  const current = stats.byEndpoint.get(endpoint) || { total: 0, failed: 0, durations: [] };
  current.total += 1;
  if (error || status >= 500) current.failed += 1;
  current.durations.push(durationMs);
  stats.byEndpoint.set(endpoint, current);
}

async function worker(workerId, stopAt) {
  while (Date.now() < stopAt) {
    const endpoint = pickEndpoint();
    const started = performance.now();
    let status = 0;
    let error;

    try {
      const response = await fetch(`${baseUrl}${endpoint}`);
      status = response.status;
      await response.arrayBuffer();
    } catch (exception) {
      error = exception;
    }

    const durationMs = performance.now() - started;
    record(endpoint, status, durationMs, error);

    if (error) {
      console.error(`[worker ${workerId}] ${endpoint} failed: ${error.message}`);
    }

    await sleep(thinkTimeMs);
  }
}

async function main() {
  const stopAt = Date.now() + durationSeconds * 1000;
  console.log(`SpeedyShop Node.js load test`);
  console.log(`Base URL: ${baseUrl}`);
  console.log(`Duration: ${durationSeconds}s, concurrency: ${concurrency}, think time: ${thinkTimeMs}ms`);

  const progress = setInterval(() => {
    const elapsedSeconds = Math.max(1, durationSeconds - Math.ceil((stopAt - Date.now()) / 1000));
    const rps = stats.total / elapsedSeconds;
    console.log(`progress: requests=${stats.total}, failed=${stats.failed}, rps=${rps.toFixed(1)}, p95=${percentile(stats.durations, 95).toFixed(0)}ms`);
  }, 5000);

  await Promise.all(Array.from({ length: concurrency }, (_, index) => worker(index + 1, stopAt)));
  clearInterval(progress);

  const totalSeconds = durationSeconds;
  console.log('\nSummary');
  console.log('=======');
  console.log(`Requests: ${stats.total}`);
  console.log(`OK: ${stats.ok}`);
  console.log(`Failed: ${stats.failed}`);
  console.log(`Requests/sec: ${(stats.total / totalSeconds).toFixed(1)}`);
  console.log(`Average: ${(stats.durations.reduce((sum, value) => sum + value, 0) / Math.max(1, stats.durations.length)).toFixed(0)}ms`);
  console.log(`P50: ${percentile(stats.durations, 50).toFixed(0)}ms`);
  console.log(`P95: ${percentile(stats.durations, 95).toFixed(0)}ms`);
  console.log(`P99: ${percentile(stats.durations, 99).toFixed(0)}ms`);

  console.log('\nBy endpoint');
  for (const [endpoint, endpointStats] of stats.byEndpoint.entries()) {
    console.log(`${endpoint} -> count=${endpointStats.total}, failed=${endpointStats.failed}, p95=${percentile(endpointStats.durations, 95).toFixed(0)}ms`);
  }
}

main().catch((error) => {
  console.error(error);
  process.exitCode = 1;
});