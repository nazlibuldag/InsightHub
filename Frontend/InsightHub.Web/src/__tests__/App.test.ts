import { describe, it, expect } from 'vitest';

describe('InsightHub Frontend Platform Tests', () => {
  it('should parse JWT payload correctly', () => {
    const fakeToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6ImFkYjY0ZDI2LTc0MTAtNGQ2OS1iZDE5LTExMWZlYTNmMGVmZiIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL2VtYWlsYWRkcmVzcyI6ImFkbWluQGluc2lnaHRodWIuY29tIn0.signature";
    const payloadBase64 = fakeToken.split('.')[1];
    const decoded = JSON.parse(atob(payloadBase64));
    
    expect(decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress']).toBe('admin@insighthub.com');
  });

  it('should format statistical summary numbers accurately', () => {
    const rawMean = 42.123456;
    const formatted = Math.round(rawMean * 100) / 100;
    expect(formatted).toBe(42.12);
  });

  it('should format ML trend forecast step labels correctly', () => {
    const stepsAhead = 5;
    const labels = Array.from({ length: stepsAhead }, (_, i) => `T+${i + 1}`);
    
    expect(labels.length).toBe(5);
    expect(labels[0]).toBe('T+1');
    expect(labels[4]).toBe('T+5');
  });
});
