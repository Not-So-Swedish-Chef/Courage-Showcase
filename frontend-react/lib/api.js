export const API_BASE_URL = 'https://your-aspnet-api.com'

export const api = {
  async post(endpoint, data) {
    const response = await fetch(`${API_BASE_URL}${endpoint}`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(data),
    })
    return handleResponse(response)
  }
}

async function handleResponse(response) {
  const data = await response.json()
  if (!response.ok) {
    throw new Error(data.message || 'Request Errors')
  }
  return data
}