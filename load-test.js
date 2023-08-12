import http from "k6/http";
import { sleep } from "k6";

export const options = {
  stages: [
    { duration: "30s", target: 100 }, // simulate ramp-up of traffic from 1 to 100 users over 30 seconds.
    { duration: "30s", target: 100 }, // stay at 100 users for 30 seconds
    { duration: "60s", target: 150 }, // stay at 150 users for 60 seconds
    { duration: "30s", target: 0 }, // ramp-down to 0 users
  ],
  thresholds: {
    http_req_duration: ["p(99)<2000"], // 99% of requests must complete below 1.5s
  },
};

const BASE_URL = "http://localhost:5265";

export default () => {
  http.get(`${BASE_URL}/api/Cities`).json();

  sleep(1);
};
