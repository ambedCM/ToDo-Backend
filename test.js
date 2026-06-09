const http = require('http');

async function request(method, path, data = null, token = null) {
  return new Promise((resolve, reject) => {
    const options = {
      hostname: 'localhost',
      port: 5011,
      path: '/api' + path,
      method: method,
      headers: {
        'Content-Type': 'application/json'
      }
    };
    if (token) options.headers['Authorization'] = 'Bearer ' + token;

    const req = http.request(options, (res) => {
      let body = '';
      res.on('data', chunk => body += chunk);
      res.on('end', () => {
        try {
          resolve({ status: res.statusCode, body: body ? JSON.parse(body) : null });
        } catch(e) {
          resolve({ status: res.statusCode, body });
        }
      });
    });

    req.on('error', reject);
    if (data) req.write(JSON.stringify(data));
    req.end();
  });
}

async function runTests() {
  try {
    const timestamp = Date.now();
    console.log('--- Registering User ---');
    const regRes = await request('POST', '/Auth/register', {
      name: `user${timestamp}`,
      email: `user${timestamp}@test.com`,
      password: "Password123!"
    });
    console.log('Register response:', regRes);

    console.log('\n--- Logging In ---');
    const loginRes = await request('POST', '/Auth/login', {
      email: `user${timestamp}@test.com`,
      password: "Password123!"
    });
    console.log('Login response:', loginRes);
    
    if (loginRes.status !== 200 || !loginRes.body.token) {
      console.log('Login failed, stopping tests');
      return;
    }
    const token = loginRes.body.token;

    console.log('\n--- Creating Task ---');
    const createRes = await request('POST', '/Task', {
      title: "Test task",
      dueDate: "2026-06-10"
    }, token);
    console.log('Create response:', createRes);
    const taskId = createRes.body.task?.id;

    console.log('\n--- Getting Tasks ---');
    const getRes = await request('GET', '/Task', null, token);
    console.log('Get response:', getRes);

    if (taskId) {
      console.log('\n--- Updating Task ---');
      const updateRes = await request('PUT', `/Task/${taskId}`, {
        title: "Updated task",
        dueDate: "2026-06-11"
      }, token);
      console.log('Update response:', updateRes);

      console.log('\n--- Completing Task ---');
      const completeRes = await request('PATCH', `/Task/${taskId}/complete`, null, token);
      console.log('Complete response:', completeRes);

      console.log('\n--- Deleting Task ---');
      const deleteRes = await request('DELETE', `/Task/${taskId}`, null, token);
      console.log('Delete response:', deleteRes);
    }
    
    console.log('\nAll API tests finished.');
  } catch(e) {
    console.error('Test error:', e);
  }
}

runTests();
