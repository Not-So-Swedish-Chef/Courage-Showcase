'use client'
import { useState } from 'react'
import { useRouter } from 'next/navigation'
import { api } from '@/lib/api'

export default function LoginPage() {
    const router = useRouter()
    const [error, setError] = useState('')
  
    const handleSubmit = async (e) => {
      e.preventDefault()
      const formData = new FormData(e.target)
      
      try {
        // 调用ASP.NET登录接口
        const response = await fetch(`${api.API_BASE_URL}/auth/login`, {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json'
          },
          body: JSON.stringify({
            email: formData.get('email'),
            password: formData.get('password')
          }),
          credentials: 'include' // 关键修改：允许跨域Cookie
        })
  
        if (response.ok) {
          // 由ASP.NET后端设置Cookie
          router.push('/')
        } else {
          const errorData = await response.json()
          setError(errorData.message)
        }
      } catch (err) {
        setError('sign in failed')
      }
    }

  return (
    <div className="flex min-h-screen items-center justify-center bg-gray-100 p-6">
      <div className="w-full max-w-md bg-white shadow-lg rounded-lg p-6">
        <h1 className="text-2xl font-bold text-center text-gray-800 mb-4">Login</h1>
        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-gray-700">Email</label>
            <input
              name="email"
              type="email"
              required
              placeholder="user@example.com"
              className="mt-1 w-full px-3 py-2 border rounded-md shadow-sm focus:ring focus:ring-indigo-200"
            />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700">Password</label>
            <input
              name="password"
              type="password"
              required
              placeholder="••••••••"
              className="mt-1 w-full px-3 py-2 border rounded-md shadow-sm focus:ring focus:ring-indigo-200"
            />
          </div>

          {error && <div className="text-red-500 text-sm">{error}</div>}

          <button
            type="submit"
            disabled={loading}
            className="w-full bg-indigo-600 text-white py-2 rounded-md hover:bg-indigo-700 transition"
          >
            {loading ? 'Logging in...' : 'Login'}
          </button>
        </form>

        <p className="mt-4 text-center text-sm text-gray-600">
          Don't have an account? <a href="/register" className="text-indigo-600 hover:underline">Sign up</a>
        </p>
      </div>
    </div>
  )
}
