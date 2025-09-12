import { create } from 'zustand'

export const useAuthStore = create((set) => ({
  user: null,
  role: null,
  isAuthenticated: false,
  
  login: (token, role) => {
    localStorage.setItem('token', token)
    localStorage.setItem('role', role)
    set({ isAuthenticated: true, role })
  },
  
  logout: () => {
    localStorage.removeItem('token')
    localStorage.removeItem('role')
    set({ isAuthenticated: false, role: null })
  },
  
  initialize: () => {
    const token = localStorage.getItem('token')
    const role = localStorage.getItem('role')
    if (token && role) {
      set({ isAuthenticated: true, role })
    }
  }
}))