using System;
using System.Diagnostics;
using Numpy;

namespace NeuralNetworkExample
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Fitting random data with a two layer Neural Network ...");
            // N is batch size; D_in is input dimension;
            // H is hidden dimension; D_out is output dimension.
            var (N, D_in, H, D_out) = (64, 1000, 100, 10);

            // Create random input and output data
            Console.WriteLine("\tcreating random data");
            var x = np.random.randn(N, D_in);
            var y = np.random.randn(N, D_out);

            Console.WriteLine("\tlearning");
            var stopwatch = Stopwatch.StartNew();
            // Randomly initialize weights
            var w1 = np.random.randn(D_in, H);
            var w2 = np.random.randn(H, D_out);

            var learning_rate = 1.0e-6;
            double loss=double.MaxValue;
            for (int t = 0; t < 500; t++)
            {
                // Forward pass: compute predicted y
                var h = x.dot(w1);
                var h_relu = np.maximum(h, (NDarray)0);
                var y_pred = h_relu.dot(w2);

                // Compute and print loss
                loss = (double)(np.square(y_pred - y).sum());
                if (t%20==0)
                    Console.WriteLine($"\tstep: {t} loss: {loss}");

                // Backprop to compute gradients of w1 and w2 with respect to loss
                var grad_y_pred = 2.0 * (y_pred - y);
                var grad_w2 = h_relu.T.dot(grad_y_pred);
                var grad_h_relu = grad_y_pred.dot(w2.T);
                var grad_h = grad_h_relu.copy();
                grad_h[h < 0] = (NDarray)0;
                var grad_w1 = x.T.dot(grad_h);

                // Update weights
                w1 -= learning_rate * grad_w1;
                w2 -= learning_rate * grad_w2;
            }
            stopwatch.Stop();
            Console.WriteLine($"\tfinal loss: {loss}, elapsed time: {stopwatch.Elapsed.TotalSeconds:F3} seconds\n");
            Console.WriteLine("Hit any key to exit.");
            Console.ReadKey();
        }
    }
}
