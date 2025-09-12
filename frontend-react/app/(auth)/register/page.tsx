'use client'

import { useState } from 'react'
import { useRouter } from 'next/navigation'
import { api } from '@/lib/api'

export default function RegisterPage() {
  const router = useRouter()
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)

  const handleSubmit = async (e:any) => {
    e.preventDefault()
    setLoading(true)
    setError('')
    
    try {
      const formData = new FormData(e.target)
      await api.post('/auth/register', {
        email: formData.get('email'),
        password: formData.get('password'),
        confirmPassword: formData.get('confirmPassword'),
        firstName: formData.get('firstName'),
        lastName: formData.get('lastName'),
        userType: formData.get('userType')
      })
      
      router.push('/login?registrationSuccess=true')
    } catch (err:any) {
      setError(err.message)
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="flex min-h-screen items-center justify-center bg-gray-100 p-6">
      <div className="w-full max-w-md bg-white shadow-lg rounded-lg p-6">
        <h1 className="text-2xl font-bold text-center text-gray-800 mb-4">Sign Up</h1>
        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-gray-700">First Name</label>
            <input name="firstName" type="text" required className="mt-1 w-full px-3 py-2 border rounded-md shadow-sm focus:ring focus:ring-indigo-200" />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700">Last Name</label>
            <input name="lastName" type="text" required className="mt-1 w-full px-3 py-2 border rounded-md shadow-sm focus:ring focus:ring-indigo-200" />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700">Email</label>
            <input name="email" type="email" required placeholder="user@example.com" className="mt-1 w-full px-3 py-2 border rounded-md shadow-sm focus:ring focus:ring-indigo-200" />
          </div>
          
          <div>
            <label className="block text-sm font-medium text-gray-700">Password</label>
            <input name="password" type="password" required placeholder="••••••••" className="mt-1 w-full px-3 py-2 border rounded-md shadow-sm focus:ring focus:ring-indigo-200" />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700">Confirm Password</label>
            <input name="confirmPassword" type="password" required placeholder="••••••••" className="mt-1 w-full px-3 py-2 border rounded-md shadow-sm focus:ring focus:ring-indigo-200" />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700">User Type</label>
            <select name="userType" required className="mt-1 w-full px-3 py-2 border rounded-md shadow-sm focus:ring focus:ring-indigo-200">
              <option value="Host">Host</option>
              <option value="Member">Member</option>
            </select>
          </div>

          {error && <div className="text-red-500 text-sm">{error}</div>}

          <button type="submit" disabled={loading} className="w-full bg-indigo-600 text-white py-2 rounded-md hover:bg-indigo-700 transition">
            {loading ? 'Registering...' : 'Sign Up'}
          </button>
        </form>
        
        <p className="mt-4 text-center text-sm text-gray-600">
          Already have an account? <a href="/login" className="text-indigo-600 hover:underline">Log in</a>
        </p>
      </div>
    </div>
  )
}
