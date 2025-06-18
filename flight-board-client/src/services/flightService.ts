
import axios from 'axios';
import { Flight, CreateFlight } from '../types/Flight';

const API_BASE_URL = 'https://localhost:32769/api'; // Update port according to your backend

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

export const flightService = {
  // Get all flights with optional filters
  getFlights: async (status?: string, destination?: string): Promise<Flight[]> => {
    const params = new URLSearchParams();
    if (status) params.append('status', status);
    if (destination) params.append('destination', destination);
    
    const response = await api.get<Flight[]>(`/flights?${params.toString()}`);
    return response.data;
  },

  // Add a new flight
  addFlight: async (flight: CreateFlight): Promise<Flight> => {
    const response = await api.post<Flight>('/flights', flight);
    return response.data;
  },

  // Delete a flight
  deleteFlight: async (id: number): Promise<void> => {
    await api.delete(`/flights/${id}`);
  },
};
