'use client'

import { useRouter } from 'next/navigation'
import { api } from '@/lib/api'
import { useAuthStore } from '@/store/authStore'

export default function AuthForm({ type }) {
  const router = useRouter()
  const { login } = useAuthStore()
  const [error, setError] = useState('')

  const handleSubmit = async (e) => {
    e.preventDefault()
    const formData = new FormData(e.target)
    
    try {
      const { data } = await api.post(`/auth/${type}`, {
        email: formData.get('email'),
        password: formData.get('password'),
        ...(type === 'register' && {
          role: formData.get('role'),
          fullName: formData.get('fullName')
        })
      })
      
      login(data.token, data.role)
      router.push('/events')
    } catch (err) {
      setError(err.response?.data?.message || 'Authentication failed')
    }
  }

  return (
    <form onSubmit={handleSubmit}>
      {type === 'register' && (
        <>
          <input name="fullName" placeholder="Full Name" required />
          <select name="role" required>
            <option value="">Select Role</option>
            <option value="Member">Member</option>
            <option value="Host">Host</option>
          </select>
        </>
      )}
      <input name="email" type="email" placeholder="Email" required />
      <input name="password" type="password" placeholder="Password" required />
      {error && <p className="error">{error}</p>}
      <button type="submit">{type === 'login' ? 'Login' : 'Register'}</button>
    </form>
  )
}